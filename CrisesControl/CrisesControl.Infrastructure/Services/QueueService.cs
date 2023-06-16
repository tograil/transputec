using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Core.Queues;
using CrisesControl.Core.Queues.Repositories;
using CrisesControl.Core.Queues.Services;
using CrisesControl.Core.Settings.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using MessageMethod = CrisesControl.SharedKernel.Enums.MessageMethod;

namespace CrisesControl.Infrastructure.Services;

public class QueueService : IQueueService
{
    private readonly CrisesControlContext _context;
    private readonly ILogger<QueueService> _logger;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IQueueRepository _queueRepository;
    private readonly IQueueMessageService _queueMessageService;
    private readonly ICompanyRepository _companyRepository;

    public QueueService(CrisesControlContext context,
        ILogger<QueueService> logger,
        ISettingsRepository settingsRepository,
        IQueueRepository queueRepository,
        IQueueMessageService queueMessageService,
        ICompanyRepository companyRepository)
    {
        _context = context;
        _logger = logger;
        _settingsRepository = settingsRepository;
        _queueRepository = queueRepository;
        _queueMessageService = queueMessageService;
        _companyRepository = companyRepository;
    }

    public async Task MessageDeviceQueue(int messageId, MessageType messageType, int priority, int cascadePlanId)
    {
        await _queueRepository.CreateMessageQueue(messageId, messageType, priority, cascadePlanId);

        await PublishMessage(messageId, 1);
    }

    public async Task<ICollection<MessageQueueItem>> GetMessageQueue(int messageId, MessageMethod method, int messageDeviceId = 0, int priority = 1)
    {
        var pMessageId = new SqlParameter("@MessageID", messageId);
        var pMethod = new SqlParameter("@Method", method.ToString());
        var pMessageDeviceId = new SqlParameter("@MessageDeviceID", messageDeviceId);
        var pPriority = new SqlParameter("@Priority", priority);

        var messageQueue = await _context.Set<MessageQueueItem>()
            .FromSqlRaw("Pro_Get_Message_Device_Queue {0}, {1}, {2}, {3}",
                pMessageId, pMethod, pMessageDeviceId, pPriority)
            .ToListAsync();

        messageQueue = messageQueue.Select(x =>
        {
            x.SenderName = string.Join(" ", x.SenderFirstName, x.SenderLastName);
            if (method == MessageMethod.Email)
            {
                x.DeviceAddress = x.UserEmail;
            }

            return x;
        }).ToList();

        _logger.LogInformation($"Queue count for {method.ToString()} is {messageQueue.Count}");

        return messageQueue;
    }

    public async Task PublishMessage(int messageId, int priority, MessageMethod method = MessageMethod.All)
    {
        var rabbitHosts = RabbitHosts();

        if (method is MessageMethod.All)
        {
            var methods = new List<MessageMethod>()
            {
                MessageMethod.Phone, MessageMethod.Push, MessageMethod.Email, MessageMethod.Phone, MessageMethod.Text
            };

            var taskList = new List<Task>();

            methods.ForEach(m => taskList.Add(EnqueueMessage(messageId, rabbitHosts, m)));

            Task.WaitAll(taskList.ToArray());
        }
        else
        {
            await EnqueueMessage(messageId, rabbitHosts, method);
        }
    }

    public async Task EnqueueMessage(int messageId, ICollection<string> rabbitHosts, MessageMethod method)
    {
        var rabbitUser = _settingsRepository.GetSetting("RABBITMQ_USER");
        var rabbitPassword = _settingsRepository.GetSetting("RABBITMQ_PASSWORD");

        var rabbitFactory = new ConnectionFactory
        {
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
            NetworkRecoveryInterval = new TimeSpan(0, 0, 15),
            RequestedHeartbeat = new TimeSpan(0, 0, 15),
            UserName = rabbitUser,
            Password = rabbitPassword
        };

        using var connection = rabbitFactory.CreateConnection(rabbitHosts.ToList());
        using var model = connection.CreateModel();

        var exchangeName = "cc_processing_exchange";

        model.ExchangeDeclare(exchangeName, "direct", true, true);

        var properties = model.CreateBasicProperties();
        properties.Persistent = true;
        properties.DeliveryMode = 2;

        var messageQueue = await GetMessageQueue(messageId, method);

        if (messageQueue.Any())
        {
            double queueSize;
            const int minItemsInQueue = 200;

            var maxQueueNumber = Convert.ToDouble(_settingsRepository.GetSetting("RABBIT_QUEUE_SIZE"));

            var deviceCount = messageQueue.Count;

            if (deviceCount <= minItemsInQueue)
            {
                queueSize = deviceCount;
            }
            else
            {
                var tmpMaxQueueNumber = decimal.ToDouble(decimal.Divide(deviceCount, minItemsInQueue));
                if (tmpMaxQueueNumber < maxQueueNumber)
                {
                    maxQueueNumber = tmpMaxQueueNumber;
                }
                queueSize = Math.Ceiling(deviceCount / maxQueueNumber);
            }

            var routingKeys = new List<string>();
            var queueItemCount = 0;
            var queueCount = 0;
            var currentPushType = string.Empty;
            string tmpRoutingKey;

            bool.TryParse(_settingsRepository.GetSetting("COMMS_DEBUG_MODE"), out var commsDebug);

            if (method is not MessageMethod.Push)
            {
                var tmpItem = new MessageQueueItem
                {
                    Status = "SKIP"
                };
                var tmpMessage = JsonConvert.SerializeObject(tmpItem);
                var tmpBody = Encoding.UTF8.GetBytes(tmpMessage);
                tmpRoutingKey = method.ToString().ToLower() + "_" + queueCount;
                model.QueueDeclare(queue: tmpRoutingKey, durable: true, exclusive: false, autoDelete: false);
                model.QueueBind(queue: tmpRoutingKey, exchange: exchangeName, routingKey: tmpRoutingKey);
                model.BasicPublish(exchange: exchangeName, routingKey: tmpRoutingKey, basicProperties: properties, body: tmpBody);
                routingKeys.Add(tmpRoutingKey);
                queueCount++;
            }

            foreach (var messageQueueItem in messageQueue)
            {
                messageQueueItem.CommsDebug = commsDebug;

                var concreteMessage = method switch
                {
                    MessageMethod.Email => _queueMessageService.GetMessage<EmailMessage>(messageQueueItem) as
                        MessageQueueItem,
                    MessageMethod.Phone => _queueMessageService.GetMessage<PhoneMessage>(messageQueueItem),
                    MessageMethod.Push => _queueMessageService.GetMessage<PushMessage>(messageQueueItem),
                    MessageMethod.Text => _queueMessageService.GetMessage<TextMessage>(messageQueueItem),
                    _ => _queueMessageService.GetMessage<EmailMessage>(messageQueueItem)
                };

                var simulationText =
                    await _companyRepository.GetCompanyParameter("INCIDENT_SIMULATION_TEXT", concreteMessage.CompanyId);
                var phoneSimulationMessage =
                    await _companyRepository.GetCompanyParameter("INCIDENT_SIMULATION_PHONE", concreteMessage.CompanyId);

                switch (concreteMessage)
                {
                    case EmailMessage emailMessage:
                        emailMessage.OneClickAcknowledge =
                            await _companyRepository.GetCompanyParameter("ONE_CLICK_EMAIL_ACKNOWLEDGE",
                                emailMessage.CompanyId);
                        emailMessage.EmailProvider =
                            await _companyRepository.GetCompanyParameter("EMAIL_PROVIDER", emailMessage.CompanyId);
                        break;
                    case PhoneMessage phoneMessage:
                        var voiceApi = await _companyRepository.GetCompanyParameter("VOICE_API", phoneMessage.CompanyId);
                   
                        phoneMessage.ClientId = voiceApi == "UNIFONIC"
                            ? await _companyRepository.GetCompanyParameter(voiceApi + "_CLIENTID", phoneMessage.CompanyId)
                            : _settingsRepository.GetSetting(voiceApi + "_CLIENTID");
                        phoneMessage.ClientSecret = _settingsRepository.GetSetting($"{voiceApi}_CLIENT_SECRET");
                        phoneMessage.FromNumber = _settingsRepository.GetSetting($"{voiceApi}_FROM_NUMBER");
                        phoneMessage.RetryNumberList =
                            await _companyRepository.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST",
                                phoneMessage.CompanyId, phoneMessage.FromNumber);
                        phoneMessage.APIClass = _settingsRepository.GetSetting($"{voiceApi}_API_CLASS");
                        phoneMessage.MessageXML = _settingsRepository.GetSetting($"{voiceApi}_MESSAGE_XML_URL");
                        phoneMessage.CallBackUrl = _settingsRepository.GetSetting($"{voiceApi}_CALLBACK_URL");

                        (phoneMessage.SendInDirect, phoneMessage.TwilioRoutingApi) = GetTwilioData();
                        break;
                    case TextMessage textMessage:
                        var smsApi = await _companyRepository.GetCompanyParameter("SMS_API", textMessage.CompanyId);
                        var messagingCopilotSid =
                            await _companyRepository.GetCompanyParameter("MESSAGING_COPILOT_SID", textMessage.CompanyId);

                        if (smsApi == "UNIFONIC")
                        {
                            textMessage.ClientId =
                                await _companyRepository.GetCompanyParameter($"{smsApi}_CLIENTID",
                                    textMessage.CompanyId);
                            textMessage.ClientSecret =
                                await _companyRepository.GetCompanyParameter($"{smsApi}_CLIENT_SECRET",
                                    textMessage.CompanyId);
                        }
                        else
                        {
                            textMessage.ClientId = _settingsRepository.GetSetting($"{smsApi}_CLIENTID");
                            textMessage.ClientSecret = _settingsRepository.GetSetting($"{smsApi}_CLIENT_SECRET");
                        }

                        textMessage.APIClass = _settingsRepository.GetSetting($"{smsApi}_API_CLASS");
                        textMessage.CallBackUrl = _settingsRepository.GetSetting($"{smsApi}_SMS_CALLBACK_URL");
                        textMessage.FromNumber = _settingsRepository.GetSetting($"{smsApi}_FROM_NUMBER");
                        textMessage.SMSAPI = smsApi;
                        textMessage.CoPilotID = messagingCopilotSid;

                        (textMessage.SendInDirect, textMessage.TwilioRoutingApi) = GetTwilioData();

                        textMessage.SendOriginalText =
                            bool.Parse(await _companyRepository.GetCompanyParameter("SEND_ORIGINAL_TEXT",
                                textMessage.CompanyId));
                        textMessage.AllowPingAckByText = bool.Parse(await _companyRepository.GetCompanyParameter("ALLOW_PING_ACK_BY_TEXT",
                            textMessage.CompanyId));
                        textMessage.AllowIncidentAckbByText = bool.Parse(await _companyRepository.GetCompanyParameter("ALLOW_INCI_ACK_BY_TEXT",
                            textMessage.CompanyId));
                        textMessage.IncludeSenderInText = bool.Parse(await _companyRepository.GetCompanyParameter("INC_SENDER_IN_SMS",
                            textMessage.CompanyId));
                        textMessage.GenericText =
                            await _companyRepository.GetCompanyParameter("MESSAGE_TEXT_GENERIC", textMessage.CompanyId);
                        textMessage.ReplyToNumber = await _companyRepository.GetCompanyParameter("REPLY_TO_NUMBER", textMessage.CompanyId,
                            textMessage.FromNumber);

                        break;
                    case PushMessage pushMessage:
                        _ = int.TryParse(
                            await _companyRepository.GetCompanyParameter("TRACKING_DURATION", pushMessage.CompanyId),
                            out var trackingDuration);
                        pushMessage.TrackingDuration = trackingDuration;

                        break;
                }

                if (concreteMessage.MessageText == "NOMSG" && concreteMessage.ParentID > 0)
                {
                    var msg = await _context.Set<Message>().FirstOrDefaultAsync(w => w.MessageId == concreteMessage.ParentID);

                    if (msg is not null)
                    {
                        concreteMessage.MessageText = msg.MessageText!;
                    }
                }

                concreteMessage.MessageText = concreteMessage.TransportType.ToUpper() switch
                {
                    "SIMULATION" when method == MessageMethod.Phone =>
                        $"{phoneSimulationMessage} {concreteMessage.MessageText}",
                    "SIMULATION" when method != MessageMethod.Phone =>
                        $"{simulationText} {concreteMessage.MessageText}",
                    _ => concreteMessage.MessageText
                };

                var routingKey = string.Empty;
                var pushType = string.Empty;

                var serializedMessage = JsonConvert.SerializeObject(concreteMessage);

                var body = Encoding.UTF8.GetBytes(serializedMessage);

                if (method == MessageMethod.Push)
                {
                    pushType = concreteMessage.DeviceType
                        switch
                        {
                            { } d when d.Contains("Android") => "android",
                            { } d when d.Contains("iPad") || d.Contains("iPhone") || d.Contains("iPod") => "ios",
                            { } d when d.Contains("WindowsDesktop") => "windowsdesktop",
                            { } d when d.Contains("Windows") => "windows",
                            { } d when d.Contains("Blackberry") => "blackberry",
                            _ => "android"
                        };

                    routingKey = $"{pushType}_push";
                }
                else
                {
                    routingKey = method.ToString().ToLower();
                }

                if (currentPushType != pushType)
                    queueCount = 0;

                tmpRoutingKey = $"{routingKey}_{queueCount}";

                if (!routingKeys.Contains(tmpRoutingKey) || queueItemCount == queueSize || currentPushType != pushType)
                {
                    queueCount++;
                    routingKey = $"{routingKey}_{queueCount}";

                    model.QueueDeclare(routingKey, true, false, false);
                    model.QueueBind(routingKey, exchangeName, routingKey);
                    routingKeys.Add(routingKey);
                }
                else
                {
                    routingKey = $"{routingKey}_{queueCount}";
                }

                model.BasicPublish(exchangeName, routingKey, properties, body);
            }
        }
    }

    private (bool sendInDirect, string twilioRoutingApi) GetTwilioData()
    {
        var sendInDirect = _settingsRepository.GetSetting("TWILIO_USE_INDIRECT_CONNECTION")
            switch
            {
                "true" => true,
                "false" => false,
                _ => false
            };
        var twilioRoutingApi = _settingsRepository.GetSetting("TWILIO_ROUTING_API");

        return (sendInDirect, twilioRoutingApi);
    }

    public ICollection<string> RabbitHosts()
    {
        var hosts = _settingsRepository.GetSetting("RABBITMQ_HOST");

        return hosts.Split(",");
    }
    public  async Task<bool> AppInvitationQueue(int CompanyID)
    {
        try
        {
            string RabbitMQUser = await _companyRepository.LookupWithKey("RABBITMQ_USER");
            string RabbitMQPassword = await _companyRepository.LookupWithKey("RABBITMQ_PASSWORD");
            ushort RabbitMQHeartBeat = Convert.ToUInt16(await _companyRepository.LookupWithKey("RABBIT_HEARTBEAT_CHECK"));
            var rabbithost = RabbitHosts();
            var factory = new ConnectionFactory()
            {
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = new TimeSpan(0, 0, 15),
                RequestedHeartbeat = TimeSpan.FromTicks(RabbitMQHeartBeat),
                UserName = RabbitMQUser,
                Password = RabbitMQPassword,
                VirtualHost = rabbithost.ToString()
            };

            using (var connection = factory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                string exchange_name = await _companyRepository.LookupWithKey("RABBITMQ_QUEUE_EXCHANGE");
                model.ExchangeDeclare(exchange: exchange_name, type: "direct", durable: true, autoDelete: false);

                IBasicProperties properties = model.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                List<MessageQueueItem> device_list = new List<MessageQueueItem>();
                device_list = await GetAppUserQueue(CompanyID);

                if (device_list.Count > 0)
                {
                    bool CommsDebug = true;
                    string EmailProvider = "";

                    var emailitem = new EmailMessage();

                    bool.TryParse(await _companyRepository.LookupWithKey("COMMS_DEBUG_MODE"), out CommsDebug);
                    emailitem =await ParamsHelper.GetEmailParams();

                    List<string> RoutingKeys = new List<string>();
                    string routingKey = "app_invitation" + "_" + CompanyID; ;

                    model.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false);
                    model.QueueBind(queue: routingKey, exchange: exchange_name, routingKey: routingKey);

                    EmailProvider = await _companyRepository.GetCompanyParameter("EMAIL_PROVIDER", CompanyID);

                    foreach (var item in device_list)
                    {
                        item.CommsDebug = CommsDebug;

                        emailitem.EmailProvider = EmailProvider;
                        string message = ParamsHelper.MergeEmailParams(emailitem, item);

                        var body = Encoding.UTF8.GetBytes(message);

                        model.BasicPublish(exchange: exchange_name, routingKey: routingKey, basicProperties: properties, body: body);
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            throw ex;
            return false;
        }
    }

    public async  Task<List<MessageQueueItem>> GetAppUserQueue(int CompanyID)
    {
       
        try
        {
            
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
            var List = await _context.Set<MessageQueueItem>().FromSqlRaw("exec Pro_App_Invite_Queue @CompanyID",
                pCompanyID).ToListAsync();
                    List.Select(c => {
                        c.SenderName = c.SenderFirstName + " " + c.SenderLastName;
                        c.DeviceAddress = c.UserEmail;
                        return c;
                    }).ToList();
                return List;
           
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return new List<MessageQueueItem>();
    }

    public async Task<List<MessageQueueItem>> GetDeviceQueue(int MessageID, string Method, int MessageDeviceId = 0, int Priority = 1)
    {
        
        try
        {
            
                var pMessageId = new SqlParameter("@MessageID", MessageID);
                var pMethod = new SqlParameter("@Method", Method);
                var pMessageDeviceId = new SqlParameter("@MessageDeviceID", MessageDeviceId);
                var pPriority = new SqlParameter("@Priority", Priority);



            var List = await _context.Set<MessageQueueItem>().FromSqlRaw("exec Pro_Get_Message_Device_Queue @MessageID,@Method,@MessageDeviceID,@Priority",
                pMessageId, pMethod, pMessageDeviceId, pPriority).ToListAsync();
                    List.Select(c => {
                        c.SenderName = c.SenderFirstName + " " + c.SenderLastName;
                        if (c.Method.ToUpper() == "EMAIL")
                        {
                            c.DeviceAddress = c.UserEmail;
                        }
                        return c;
                    }).ToList();

                Console.WriteLine("Queue Count for " + Method + " is " + List.Count);
                return List;
           
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return new List<MessageQueueItem>();
    }
    public async Task<List<MessageQueueItem>> GetFailedDeviceQueue(int MessageID, string Method, int MessageDeviceId = 0)
    {
       
        try
        {
            
                var pMessageId = new SqlParameter("@MessageID", MessageID);
                var pMethod = new SqlParameter("@Method", Method);
                var pMessageDeviceId = new SqlParameter("@MessageDeviceID", MessageDeviceId);
                
                var List = await _context.Set<MessageQueueItem>().FromSqlRaw("exec Pro_Get_Failed_Device_Queue @MessageID,@Method,@MessageDeviceID",
                    pMessageId, pMethod, pMessageDeviceId).ToListAsync();
                return List;
         
        }
        catch (Exception ex)
        {
            throw ex;
        }
        return new List<MessageQueueItem>();
    }


 

}