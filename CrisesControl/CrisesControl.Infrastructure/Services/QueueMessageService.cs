using System;
using AutoMapper;
using CrisesControl.Core.Queues;
using CrisesControl.Core.Queues.Services;
using CrisesControl.Core.Settings.Repositories;
using CrisesControl.Core.Messages;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Queues;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Messages.Services;

namespace CrisesControl.Infrastructure.Services;

public class QueueMessageService : IQueueMessageService
{
    private readonly IMapper _mapper;
    private readonly ISettingsRepository _settingsRepository;
    static string ServiceHost = string.Empty;
    public static string RabbitVirtualHost = "/";
    public static CrisesControlContext db;
    private static IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
    private static IDBCommonRepository _DBC;
    private static IMessageService _MSG;

    public QueueMessageService(IMapper mapper,
        ISettingsRepository settingsRepository)
    {
        _mapper = mapper;
        _settingsRepository = settingsRepository;
    }

    public T GetMessage<T>(MessageQueueItem baseMessage) where T : MessageQueueItem
    {
        var initialValue = _mapper.Map<T>(baseMessage);

        switch (initialValue)
        {
            case TextMessage textMessage:
                FillText(ref textMessage);
                break;
            case EmailMessage emailMessage:
                FillEmail(ref emailMessage);
                break;
            case PushMessage pushMessage:
                FillPush(ref pushMessage);
                break;
            case PhoneMessage phoneMessage:
                FillPhone(ref phoneMessage);
                break;
        }

        return initialValue;
    }

    private void FillText(ref TextMessage baseText)
    {
        baseText.TaskURL = _settingsRepository.GetSetting("INCIDENT_TASK_URL");
        baseText.TaskURLLabel = _settingsRepository.GetSetting("TASK_URL_LABEL");

        baseText.UseCopilot = Convert.ToBoolean(_settingsRepository.GetSetting("USE_MESSAGING_COPILOT"));
    }

    private void FillEmail(ref EmailMessage baseEmail)
    {
        baseEmail.TwitterPage = _settingsRepository.GetSetting("CC_TWITTER_PAGE");
        baseEmail.TwitterIcon = _settingsRepository.GetSetting("CC_TWITTER_ICON");
        baseEmail.FacebookPage = _settingsRepository.GetSetting("CC_FB_PAGE");
        baseEmail.FacebookIcon = _settingsRepository.GetSetting("CC_FB_ICON");
        baseEmail.LinkedInPage = _settingsRepository.GetSetting("CC_LINKEDIN_PAGE");
        baseEmail.LinkedInIcon = _settingsRepository.GetSetting("CC_LINKEDIN_ICON");
        baseEmail.Domain = _settingsRepository.GetSetting("DOMAIN");
        baseEmail.PortalURL = _settingsRepository.GetSetting("PORTAL");
        baseEmail.ACKUrl = _settingsRepository.GetSetting("ACKNOWLEDGE_URL");
        baseEmail.SupportEmail = _settingsRepository.GetSetting("CC_USER_SUPPORT_LINK");
        baseEmail.CCLogo = _settingsRepository.GetSetting("CCLOGO");
        baseEmail.TemplatePath = _settingsRepository.GetSetting("COMMS_TEMPLATE_PATH");
        baseEmail.EmailFrom = _settingsRepository.GetSetting("ALERT_EMAILFROM");
        baseEmail.SMTPHost = _settingsRepository.GetSetting("SMTPHOST");
        baseEmail.EmailSub = _settingsRepository.GetSetting("EMAILSUB");
        baseEmail.TaskURL = _settingsRepository.GetSetting("INCIDENT_TASK_URL");
        baseEmail.TaskURLLabel = _settingsRepository.GetSetting("TASK_URL_LABEL");
        baseEmail.SendGridAPIKey = _settingsRepository.GetSetting("SEND_GRID_API_KEY");
        baseEmail.AwsSesAccessKey = _settingsRepository.GetSetting("AWS_SES_ACCCESS_KEY");
        baseEmail.AWSSESSecretKey = _settingsRepository.GetSetting("AWS_SES_SECRET_KEY");
        baseEmail.Office365Host = _settingsRepository.GetSetting("OFFICE365_HOST");
    }

    private void FillPush(ref PushMessage basePush)
    {
        basePush.AppleCertPwd = _settingsRepository.GetSetting("APPLECERPWD");
        basePush.AppleCertPath = _settingsRepository.GetSetting("APPLECERTPATH");
        basePush.ApplePushMode = Convert.ToBoolean(_settingsRepository.GetSetting("APPLEPUSHMODE"));
        basePush.GoogleApiKey = _settingsRepository.GetSetting("GOOGLE_API_KEY");
        basePush.SoundFileName = _settingsRepository.GetSetting("PUSH_SOUND_FILE");
        basePush.WinPackageName = _settingsRepository.GetSetting("WINAPP_PACKAGE_NAME");
        basePush.WinAppSID = _settingsRepository.GetSetting("WINAPP_SID");
        basePush.WinClientSecret = _settingsRepository.GetSetting("WINAPP_CLIENT_SECRET");
        basePush.WinDeskPackageName = _settingsRepository.GetSetting("WINDESK_PACKAGE_NAME");
        basePush.WinAppSID = _settingsRepository.GetSetting("WINDESK_SID");
        basePush.WinClientSecret = _settingsRepository.GetSetting("WINDESK_CLIENT_SECRET");
    }

    private void FillPhone(ref PhoneMessage basePhone)
    {
        //TODO: Nothing to fill. Find
    }
    public async Task MessageDeviceQueue(int messageID, string messageType, int priority, int cascadePlanID = 0)
    {

        try
        {

            var pMessageId = new SqlParameter("@MessageID", messageID);
            var pMessageType = new SqlParameter("@MessageType", messageType);
            var pPriority = new SqlParameter("@Priority", priority);
            //db.Database.GetCommandTimeout();

            string sp_name = "Pro_Create_Message_Queue ";

            if (cascadePlanID > 0)
                sp_name = "Pro_Create_Message_Queue_Cascading ";

            // _DBC.LocalException(MessageID.ToString(), sp_name, Priority.ToString());

            var List = await db.Database.ExecuteSqlRawAsync(sp_name + " @MessageID, @MessageType, @Priority", pMessageId, pMessageType, pPriority);

           await Task.Factory.StartNew(() => MessageDevicePublish(messageID, 1, ""));

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task MessageDevicePublish(int messageID, int priority, string method = "")
    {

        try
        {
            var rabbithosts = RabbitHosts(out RabbitVirtualHost);
            List<Task> queuetask = new List<Task>();
            //Task temail = null; Task tpush = null; Task tphone = null; Task ttext = null; Task twhatsapp = null;

            if (method == "" || method == "EMAIL")
            {
                //Console.WriteLine("Publishing messages for " + Method);
                var temail = await Task.Factory.StartNew(() => PublishMessageQueue(messageID, rabbithosts, "EMAIL", null, priority));
                if (temail != null)
                    queuetask.Add(temail);
            }


            if (method == "" || method == "PUSH")
            {
                //Console.WriteLine("Publishing messages for " + Method);
                var tpush = Task.Factory.StartNew(() => PublishMessageQueue(messageID, rabbithosts, "PUSH", null, priority));
                if (tpush != null)
                    queuetask.Add(tpush);
            }


            if (method == "" || method == "PHONE")
            {
                //Console.WriteLine("Publishing messages for " + Method);
                var tphone = Task.Factory.StartNew(() => PublishMessageQueue(messageID, rabbithosts, "PHONE", null, priority));
                if (tphone != null)
                    queuetask.Add(tphone);
            }


            if (method == "" || method == "TEXT")
            {
                //Console.WriteLine("Publishing messages for " + Method);
                var ttext = Task.Factory.StartNew(() => PublishMessageQueue(messageID, rabbithosts, "TEXT", null, priority));
                if (ttext != null)
                    queuetask.Add(ttext);
            }


            //if (Method == "" || Method == "WHATSAPP") {
            //    //Console.WriteLine("Publishing messages for " + Method);
            //    twhatsapp = Task.Factory.StartNew(() => PublishMessageQueue(MessageID, rabbithosts, "WHATSAPP", null, Priority)).ContinueWith(t => {
            //        t.Dispose();
            //        //DBC.CreateLog("INFO", "WHATSAPP Queue Disposed for messageid " + MessageID);
            //    });
            //}
            //if (twhatsapp != null)
            //    queuetask.Add(twhatsapp);

            Task[] tasksArray = queuetask.Where(t => t != null).ToArray();

            if (tasksArray.Length > 0)
            {
                Task.WaitAll(tasksArray.ToArray());
                foreach (Task tsk in tasksArray)
                {
                    if (tsk != null)
                    {
                        if (tsk.IsCompleted)
                        {
                            //DBC.CreateLog("INFO", tsk.Id + " for messageid " + MessageID + " is " + tsk.Status);
                            if (tsk != null)
                                tsk.Dispose();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public async Task<bool> PublishMessageQueue(int messageID, List<string> rabbitHost, string method,
       dynamic devicelist = null, int priority = 1)
    {


        try
        {
            await _MSG.CreateProcessQueue(messageID, "", method, "CONFIRM", 99);

            string RabbitMQUser = await _DBC.LookupWithKey("RABBITMQ_USER");
            string RabbitMQPassword = await _DBC.LookupWithKey("RABBITMQ_PASSWORD");
            string RabbitVirtualHosts = await _DBC.LookupWithKey("RABBITMQ_VIRTUAL_HOST");
            ushort RabbitMQHeartBeat = Convert.ToUInt16(await _DBC.LookupWithKey("RABBIT_HEARTBEAT_CHECK"));
            var factory = new ConnectionFactory()
            {
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = new TimeSpan(0, 0, 15),
                RequestedHeartbeat = TimeSpan.FromTicks(RabbitMQHeartBeat),
                UserName = RabbitMQUser,
                Password = RabbitMQPassword,
                VirtualHost = RabbitVirtualHosts
            };

            using (var connection = factory.CreateConnection(rabbitHost))
            using (var model = connection.CreateModel())
            {
                string exchange_name = await _DBC.LookupWithKey("RABBITMQ_QUEUE_EXCHANGE");
                model.ExchangeDeclare(exchange: exchange_name, type: "direct", durable: true, autoDelete: false);

                IBasicProperties properties = model.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                List<MessageQueueItem> device_list = new List<MessageQueueItem>();
                if (devicelist != null)
                {
                    device_list = devicelist;
                }
                else
                {
                    device_list = await  GetDeviceQueue(messageID, method, 0, priority);
                }

                if (device_list.Count > 0)
                {
                    bool OneTimeParams = false, SEND_ORIGINAL_TEXT = false,
                        ALLOW_ACKNOWLEDGE_PING = true, ALLOW_ACKNOWLEDGE_INCIDENT = true, INCLUDE_SENDER_IN_SMS = true, CommsDebug = true,
                        AllowPingAckByWA = false, AllowIncidentAckbByWA = false, IncludeSenderInWA = false, SendInDirect = false;

                    double QueueSize = 100;

                    string VoiceClientId = "", VoiceClientSecret = "", SMSClientId = "", SMSClientSecret = "", FromNumber = "", RetryNumberList = "",
                        CallBackUrl = "", VoiceAPIClass = "", SMSAPIClass = "", MessageXML = "", OneClickAck = "", EmailProvider = "", TextMessageXML = "", GENERIC_TEXT = "",
                        SIMULATION_TEXT = "", PHONE_SIMULATION_MESSAGE = "", TwilioRoutingApi = "", ReplyToNumber = "";

                    List<string> FromNumberList = new List<string>();

                    List<AckOption> newAckOption = new List<AckOption>();

                    int MAXMSGATTEMPT = 2, TrackingDuration = 30;

                    var emailitem = new EmailMessage();
                    var pushitem = new PushMessage();
                    var phoneitem = new PhoneMessage();
                    var textitem = new TextMessage();
                    var waitem = new TextMessage();


                    int MinItemInQueue = 200;

                    bool.TryParse(await _DBC.LookupWithKey("COMMS_DEBUG_MODE"), out CommsDebug);
                    SendInDirect = await _DBC.IsTrue(await _DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                    MAXMSGATTEMPT = Convert.ToInt32(await _DBC.LookupWithKey("MAXMSGATTEMPT"));
                    double MaxQueueNumber = Convert.ToDouble(await _DBC.LookupWithKey("RABBIT_QUEUE_SIZE"));
                    int DeviceCount = device_list.Count;


                    if (DeviceCount <= MinItemInQueue)
                    {
                        QueueSize = Math.Ceiling((double)(DeviceCount / 1));
                    }
                    else
                    {
                        double TmpMaxQueueNumber = DeviceCount / MinItemInQueue;
                        if (TmpMaxQueueNumber < MaxQueueNumber)
                        {
                            MaxQueueNumber = TmpMaxQueueNumber;
                            //Math.Ceiling((double)(DeviceCount / MinItemInQueue));
                        }
                        QueueSize = Math.Ceiling(DeviceCount / MaxQueueNumber);
                    }


                    //DBC.CreateLog("ERROR", "qUEUE sIZE" + QueueSize);

                    TwilioRoutingApi = await _DBC.LookupWithKey("TWILIO_ROUTING_API");

                    if (method.ToUpper() == "EMAIL")
                    {
                        emailitem = await ParamsHelper.GetEmailParams();
                    }
                    else if (method.ToUpper() == "PUSH")
                    {
                        pushitem = await ParamsHelper.GetPushParams();
                    }
                    else if (method.ToUpper() == "TEXT")
                    {
                        textitem = await ParamsHelper.GetTextParams();
                    }
                    else if (method.ToUpper() == "PHONE")
                    {
                        phoneitem = ParamsHelper.GetPhoneParams();
                    }
                    else if (method.ToUpper() == "WHATSAPP")
                    {
                        waitem = await ParamsHelper.GetWhatsAppParams();
                    }


                    List<string> RoutingKeys = new List<string>();
                    string routingKey = "";
                    string push_type = "";

                    int PublishCount = 0;
                    int QueueItemCount = 0;
                    int QueueCount = 0;
                    string CurrentPushType = "";
                    string tmproutingKey = "";

                    if (method.ToUpper() != "PUSH")
                    {
                        MessageQueueItem tmpitem = new MessageQueueItem();
                        tmpitem.Status = "SKIP";
                        string tmpmessage = JsonConvert.SerializeObject(tmpitem);
                        var tmpbody = Encoding.UTF8.GetBytes(tmpmessage);
                        tmproutingKey = method.ToLower() + "_" + QueueCount;

                        try
                        {
                            model.QueueDeclare(queue: tmproutingKey, durable: true, exclusive: false, autoDelete: false);
                            model.QueueBind(queue: tmproutingKey, exchange: exchange_name, routingKey: tmproutingKey);
                            model.BasicPublish(exchange: exchange_name, routingKey: tmproutingKey, basicProperties: properties, body: tmpbody);
                            RoutingKeys.Add(tmproutingKey);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        QueueCount++;
                    }

                    foreach (var item in device_list)
                    {
                        item.CommsDebug = CommsDebug;

                        if ((string.IsNullOrEmpty(item.ISDCode) || string.IsNullOrEmpty(item.MobileNo))
                            && (method.ToUpper() == "PHONE" || method.ToUpper() == "TEXT"))
                        {
                            continue;
                        }

                        //if((Method.ToUpper() == "TEXT" || Method.ToUpper() == "PHONE" || Method.ToUpper() == "WHATSAPP") && item.MobileNo.Length < 4) {
                        //    continue;
                        //}

                        if (!OneTimeParams)
                        {
                            SIMULATION_TEXT = await _DBC.GetCompanyParameter("INCIDENT_SIMULATION_TEXT", item.CompanyId);
                            PHONE_SIMULATION_MESSAGE = await _DBC.GetCompanyParameter("INCIDENT_SIMULATION_PHONE", item.CompanyId);


                            if (method.ToUpper() == "EMAIL")
                            {
                                OneClickAck = await _DBC.GetCompanyParameter("ONE_CLICK_EMAIL_ACKNOWLEDGE", item.CompanyId);
                                EmailProvider = await _DBC.GetCompanyParameter("EMAIL_PROVIDER", item.CompanyId);
                                emailitem.OneClickAcknowledge = OneClickAck;
                                emailitem.EmailProvider = EmailProvider;

                            }
                            else if (method.ToUpper() == "PHONE" || method.ToUpper() == "TEXT")
                            {
                                if (method.ToUpper() == "PHONE")
                                {
                                    string VOICE_API = await _DBC.GetCompanyParameter("VOICE_API", item.CompanyId);
                                    if (VOICE_API == "UNIFONIC")
                                    {
                                        VoiceClientId = await _DBC.GetCompanyParameter(VOICE_API + "_PHONE_CLIENTID", item.CompanyId);
                                    }
                                    else
                                    {
                                        VoiceClientId = await _DBC.LookupWithKey(VOICE_API + "_CLIENTID");
                                    }

                                    VoiceClientSecret = await _DBC.LookupWithKey(VOICE_API + "_CLIENT_SECRET");
                                    FromNumber = await _DBC.LookupWithKey(VOICE_API + "_FROM_NUMBER");
                                    CallBackUrl = await _DBC.LookupWithKey(VOICE_API + "_CALLBACK_URL");
                                    VoiceAPIClass = await _DBC.LookupWithKey(VOICE_API + "_API_CLASS");
                                    MessageXML = await _DBC.LookupWithKey(VOICE_API + "_MESSAGE_XML_URL");
                                    RetryNumberList = await _DBC.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST", item.CompanyId, FromNumber);
                                    FromNumberList = RetryNumberList.Split(',').ToList();
                                    phoneitem.VoiceAPI = VOICE_API;
                                }

                                if (method.ToUpper() == "TEXT")
                                {
                                    string SMS_API = await _DBC.GetCompanyParameter("SMS_API", item.CompanyId);
                                    string MESSAGING_COPILOT_SID = await _DBC.GetCompanyParameter("MESSAGING_COPILOT_SID", item.CompanyId);
                                    if (SMS_API == "UNIFONIC")
                                    {
                                        SMSClientId = await _DBC.GetCompanyParameter(SMS_API + "_CLIENTID", item.CompanyId);
                                        SMSClientSecret = await _DBC.GetCompanyParameter(SMS_API + "_CLIENT_SECRET", item.CompanyId);
                                    }
                                    else
                                    {
                                        SMSClientId = await _DBC.LookupWithKey(SMS_API + "_CLIENTID");
                                        SMSClientSecret = await _DBC.LookupWithKey(SMS_API + "_CLIENT_SECRET");
                                    }

                                    SMSAPIClass = await _DBC.LookupWithKey(SMS_API + "_API_CLASS");
                                    TextMessageXML = await _DBC.LookupWithKey(SMS_API + "_SMS_CALLBACK_URL");
                                    FromNumber = await _DBC.LookupWithKey(SMS_API + "_FROM_NUMBER");
                                    textitem.SMSAPI = SMS_API;
                                    textitem.CoPilotID = MESSAGING_COPILOT_SID;
                                }

                                textitem.ClientId = SMSClientId; textitem.ClientSecret = SMSClientSecret; textitem.FromNumber = FromNumber;
                                textitem.APIClass = SMSAPIClass; textitem.CallBackUrl = TextMessageXML; textitem.SendInDirect = SendInDirect;
                                textitem.TwilioRoutingApi = TwilioRoutingApi;

                                phoneitem.ClientId = VoiceClientId; phoneitem.ClientSecret = VoiceClientSecret; phoneitem.FromNumber = FromNumber; phoneitem.RetryNumberList = RetryNumberList;
                                phoneitem.APIClass = VoiceAPIClass; phoneitem.MessageXML = MessageXML; phoneitem.CallBackUrl = CallBackUrl;
                                phoneitem.SendInDirect = SendInDirect; phoneitem.TwilioRoutingApi = TwilioRoutingApi;
                            }

                            if (method.ToUpper() == "TEXT" || method.ToUpper() == "WHATSAPP")
                            {
                                SEND_ORIGINAL_TEXT = Convert.ToBoolean(_DBC.GetCompanyParameter("SEND_ORIGINAL_TEXT", item.CompanyId));
                                ReplyToNumber = await _DBC.GetCompanyParameter("REPLY_TO_NUMBER", item.CompanyId);

                                GENERIC_TEXT = await _DBC.GetCompanyParameter("MESSAGE_TEXT_GENERIC", item.CompanyId);

                                if (method.ToUpper() == "WHATSAPP")
                                {
                                    AllowPingAckByWA = Convert.ToBoolean(await _DBC.GetCompanyParameter("SEND_ORIGINAL_WATEXT", item.CompanyId)); ;
                                    AllowIncidentAckbByWA = Convert.ToBoolean(await _DBC.GetCompanyParameter("SEND_ORIGINAL_WATEXT", item.CompanyId)); ;
                                    IncludeSenderInWA = Convert.ToBoolean(await _DBC.GetCompanyParameter("SEND_ORIGINAL_WATEXT", item.CompanyId)); ;
                                    waitem.AllowIncidentAckbByWA = AllowIncidentAckbByWA; waitem.AllowPingAckByWA = AllowPingAckByWA;
                                    waitem.IncludeSenderInWA = IncludeSenderInWA;
                                }
                                else
                                {
                                    ALLOW_ACKNOWLEDGE_PING = Convert.ToBoolean(_DBC.GetCompanyParameter("ALLOW_PING_ACK_BY_TEXT", item.CompanyId));
                                    ALLOW_ACKNOWLEDGE_INCIDENT = Convert.ToBoolean(_DBC.GetCompanyParameter("ALLOW_INCI_ACK_BY_TEXT", item.CompanyId));
                                    INCLUDE_SENDER_IN_SMS = Convert.ToBoolean(_DBC.GetCompanyParameter("INC_SENDER_IN_SMS", item.CompanyId));
                                    textitem.SendOriginalText = SEND_ORIGINAL_TEXT; textitem.AllowPingAckByText = ALLOW_ACKNOWLEDGE_PING;
                                    textitem.AllowIncidentAckbByText = ALLOW_ACKNOWLEDGE_INCIDENT; textitem.IncludeSenderInText = INCLUDE_SENDER_IN_SMS;
                                    textitem.GenericText = GENERIC_TEXT; textitem.ReplyToNumber = ReplyToNumber;
                                }
                            }
                            newAckOption = await _DBC.GetAckOptions(messageID);
                            OneTimeParams = true;
                        }
                        else if (method.ToUpper() == "PUSH")
                        {
                            int.TryParse(await _DBC.GetCompanyParameter("TRACKING_DURATION", item.CompanyId), out TrackingDuration);
                            pushitem.TrackingDuration = TrackingDuration;
                        }

                        if (item.MessageText == "NOMSG" && item.ParentID > 0)
                        {

                            string msg = await db.Set<Message>().Where(w => w.MessageId == item.ParentID).Select(s => s.MessageText).FirstOrDefaultAsync(); ;
                            if (msg != null)
                            {
                                item.MessageText = msg;
                            }

                        }


                        if (item.TransportType.ToUpper() == "SIMULATION" && method.ToUpper() == "PHONE")
                        {
                            item.MessageText = PHONE_SIMULATION_MESSAGE + " " + item.MessageText;
                        }
                        else if (item.TransportType.ToUpper() == "SIMULATION" && method.ToUpper() != "PHONE")
                        {
                            item.MessageText = SIMULATION_TEXT + " " + item.MessageText;
                        }
                        else
                        {
                            item.MessageText = item.MessageText;
                        }


                        string message = "";
                        if (method.ToUpper() == "EMAIL")
                        {
                            message = ParamsHelper.MergeEmailParams(emailitem, item);
                        }
                        else if (method.ToUpper() == "PHONE")
                        {
                            phoneitem.FromNumber = await _DBC.GetValueByIndex(FromNumberList, item.Attempt);
                            message = ParamsHelper.MergePhoneParams(phoneitem, item);
                        }
                        else if (method.ToUpper() == "TEXT")
                        {
                            message = ParamsHelper.MergeTextParams(textitem, item);
                        }
                        else if (method.ToUpper() == "PUSH")
                        {
                            pushitem.AckOptions = newAckOption;
                            message = ParamsHelper.MergePushParams(pushitem, item);
                        }
                        else if (method.ToUpper() == "WHATSAPP")
                        {
                            message = ParamsHelper.MergeWAParams(waitem, item);
                        }


                        var body = Encoding.UTF8.GetBytes(message);

                        if (method.ToUpper() == "PUSH")
                        {
                            if (item.DeviceType.Contains("Android"))
                            {
                                push_type = "android";
                            }
                            else if (item.DeviceType.Contains("iPad") || item.DeviceType.Contains("iPhone") || item.DeviceType.Contains("iPod"))
                            {
                                push_type = "ios";
                            }
                            else if (item.DeviceType.Contains("WindowsDesktop"))
                            {
                                push_type = "windowsdesktop";
                            }
                            else if (item.DeviceType.Contains("Windows"))
                            {
                                push_type = "windows";
                            }
                            else if (item.DeviceType.Contains("Blackberry"))
                            {
                                push_type = "blackberry";
                            }
                            //routingKey = Method.ToLower() + "_" + push_type + "_" + MessageID;
                            routingKey = push_type + "_push";
                        }
                        else
                        {
                            //routingKey = Method.ToLower() + "_" + MessageID;
                            routingKey = method.ToLower();
                        }

                        if (CurrentPushType != push_type)
                            QueueCount = 0;

                        tmproutingKey = routingKey + "_" + QueueCount;

                        if (!RoutingKeys.Contains(tmproutingKey) || QueueItemCount == QueueSize || CurrentPushType != push_type)
                        {
                            //Dictionary<string, object> args = new Dictionary<string, object>();
                            //args.Add("x-queue-mode", "lazy");
                            QueueCount++;
                            routingKey = routingKey + "_" + QueueCount;
                            //var rqueue = model.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false, arguments: args);
                            try
                            {
                                model.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false);
                                model.QueueBind(queue: routingKey, exchange: exchange_name, routingKey: routingKey);
                                RoutingKeys.Add(routingKey);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }

                            await _DBC.MessageProcessLog(messageID, "MESSAGE_QUEUE_PUBLISHING", method, routingKey, "Count: " + QueueItemCount);

                            QueueItemCount = 0;
                        }
                        else
                        {
                            routingKey = routingKey + "_" + QueueCount;
                        }

                        CurrentPushType = push_type;

                        try
                        {
                            model.BasicPublish(exchange: exchange_name, routingKey: routingKey, basicProperties: properties, body: body);
                            PublishCount++;
                            QueueItemCount++;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    await _DBC.MessageProcessLog(messageID, "MESSAGE_QUEUE_PUBLISHED", method, "", "Total published: " + PublishCount);
                }
            }
            return true;
        }
        catch (Exception ex)
        {

            await _MSG.CreateProcessQueue(messageID, "", method, "REQUEUE", 999);
            // MessageHelpers.Common CMN = new MessageHelpers.Common();
            // CMN.NotifyRabbitServiceFailure(ex, "Critical!!: " + Method + MessageID + " Queue was not published, please check the system immediatly", false);
            return false;
        }
    }
    public List<string> RabbitHosts(out string VirtualHost)
    {

        string RabbitHost = _DBC.LookupWithKey("RABBITMQ_HOST").ToString();
        VirtualHost = _DBC.LookupWithKey("RABBITMQ_VIRTUAL_HOST").ToString();

        List<string> hostlist = RabbitHost.Split(',').ToList();
        return hostlist;
    }
    public async Task<List<MessageQueueItem>> GetDeviceQueue(int messageID, string method, int messageDeviceId = 0, int priority = 1)
    {

        try
        {

            var pMessageId = new SqlParameter("@MessageID", messageID);
            var pMethod = new SqlParameter("@Method", method);
            var pMessageDeviceId = new SqlParameter("@MessageDeviceID", messageDeviceId);
            var pPriority = new SqlParameter("@Priority", priority);


            db.Database.GetCommandTimeout();
            var List = db.Set<MessageQueueItem>().FromSqlRaw("exec Pro_Get_Message_Device_Queue @MessageID,@Method,@MessageDeviceID,@Priority",
                pMessageId, pMethod, pMessageDeviceId, pPriority).ToList().Select(c => {
                    c.SenderName = c.SenderFirstName + " " + c.SenderLastName;
                    if (c.Method.ToUpper() == "EMAIL")
                    {
                        c.DeviceAddress = c.UserEmail;
                    }
                    return c;
                }).ToList();

            Console.WriteLine("Queue Count for " + method + " is " + List.Count);
            return List;

        }
        catch (Exception ex)
        {
            throw ex;
        }
        return new List<MessageQueueItem>();
    }

    public async Task<dynamic> GetFailedDeviceQueue(int messageId, string method, int messageDeviceId = 0)
    {
        try
        {
            var pMessageId = new SqlParameter("@MessageID", messageId);
            var pMethod = new SqlParameter("@Method", method);
            var pMessageDeviceId = new SqlParameter("@MessageDeviceID", messageDeviceId);
            db.Database.SetCommandTimeout(300);
            var List = await db.Set<FailedDeviceQueue>().FromSqlRaw("EXEC Pro_Get_Failed_Device_Queue @MessageID,@Method,@MessageDeviceID",
                pMessageId, pMethod, pMessageDeviceId).ToListAsync();
            return List;
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    public async Task<List<string>> RabbitHosts()
    {

        string rabbitHost = await  _DBC.LookupWithKey("RABBITMQ_HOST");
        List<string> hostlist = rabbitHost.Split(',').ToList();
        return hostlist;
    }

    public async Task<bool> RequeueMessage(int messageId, string method, dynamic devicelist)
    {
        var rabbithosts = await  RabbitHosts();
        return await PublishMessageQueue(messageId, rabbithosts, method, devicelist);
    }

    public async Task<List<MessageQueueItem>> GetPublicAlertDeviceQueue(int messageId, string method)
    {

        try
        {
            var pMessageId = new SqlParameter("@MessageID", messageId);
            var pMethod = new SqlParameter("@Method", method);

            db.Database.SetCommandTimeout(300);
            var List = await db.Set<MessageQueueItem>().FromSqlRaw("EXEC Get_Public_Alert_Queue @MessageID, @Method", pMessageId, pMethod).ToListAsync();


                List.Select(c => {
                c.SenderName = c.SenderFirstName + " " + c.SenderLastName;
                return c;
            }).ToList();
            return List;
        }
        catch (Exception ex)
        {
        }
        return new List<MessageQueueItem>();

    }

    public async Task<bool> PublishPublicAlertQueue(int messageId, List<string> rabbitHost, string method)
    {


        try
        {

            string RabbitMQUser = await _DBC.LookupWithKey("RABBITMQ_USER");
            string RabbitMQPassword = await _DBC.LookupWithKey("RABBITMQ_PASSWORD");

            // DBC.CreateLog("INFO", "Step 3" + RabbitMQUser + RabbitMQPassword);

            var factory = new ConnectionFactory()
            {
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = new TimeSpan(0, 0, 15),
                RequestedHeartbeat = TimeSpan.FromMinutes(15),
                UserName = RabbitMQUser,
                Password = RabbitMQPassword
            };
            // DBC.CreateLog("INFO", "Step 4" + string.Join(",", RabbitHost.ToArray()));

            using (var connection = factory.CreateConnection(rabbitHost))
            using (var model = connection.CreateModel())
            {
                string exchange_name = "cc_processing_exchange";
                model.ExchangeDeclare(exchange: exchange_name, type: "direct", durable: true, autoDelete: false);

                IBasicProperties properties = model.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                List<MessageQueueItem> device_list = new List<MessageQueueItem>();

                device_list =await GetPublicAlertDeviceQueue(messageId, method);

                // DBC.CreateLog("INFO", "Step 6: " + device_list.Count);

                if (device_list.Count > 0)
                {
                    bool OneTimeParams = false, INCLUDE_SENDER_IN_SMS = true, CommsDebug = true, SendInDirect = false;

                    string SMSClientId = "", SMSClientSecret = "", FromNumber = "", SMSAPIClass = "", OneClickAck = "", EmailProvider = "",
                         TextMessageXML = "", TwilioRoutingApi = "";

                    List<AckOption> newAckOption = new List<AckOption>();

                    int MAXMSGATTEMPT = 2;

                    var emailitem = new EmailMessage();
                    var textitem = new TextMessage();

                    bool.TryParse(await _DBC.LookupWithKey("COMMS_DEBUG_MODE"), out CommsDebug);
                    SendInDirect = await _DBC.IsTrue(await _DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                    MAXMSGATTEMPT = Convert.ToInt32(_DBC.LookupWithKey("MAXMSGATTEMPT"));

                    TwilioRoutingApi = await _DBC.LookupWithKey("TWILIO_ROUTING_API");

                    emailitem = await ParamsHelper.GetEmailParams();
                    textitem = await ParamsHelper.GetTextParams();


                    List<string> RoutingKeys = new List<string>();
                    string routingKey = "";

                    int QueueCount = 0;

                    foreach (var item in device_list)
                    {
                        item.CommsDebug = CommsDebug;

                        if (string.IsNullOrEmpty(item.MobileNo) && (method.ToUpper() == "PHONE" || method.ToUpper() == "TEXT"))
                        {
                            continue;
                        }

                        if (!OneTimeParams)
                        {
                            // DBC.CreateLog("INFO", "Step 9");

                            if (item.Method.ToUpper() == "EMAIL")
                            {
                                OneClickAck = await _DBC.GetCompanyParameter("ONE_CLICK_EMAIL_ACKNOWLEDGE", item.CompanyId);
                                EmailProvider = await _DBC.GetCompanyParameter("EMAIL_PROVIDER", item.CompanyId);
                                emailitem.OneClickAcknowledge = OneClickAck;
                                emailitem.EmailProvider = EmailProvider;

                            }
                            else if (item.Method.ToUpper() == "TEXT")
                            {

                                if (method.ToUpper() == "TEXT")
                                {
                                    string SMS_API = await _DBC.GetCompanyParameter("SMS_API", item.CompanyId);
                                    string MESSAGING_COPILOT_SID = await _DBC.GetCompanyParameter("MESSAGING_COPILOT_SID", item.CompanyId);
                                    SMSClientId = await _DBC.LookupWithKey(SMS_API + "_CLIENTID");
                                    SMSClientSecret = await _DBC.LookupWithKey(SMS_API + "_CLIENT_SECRET");
                                    SMSAPIClass = await _DBC.LookupWithKey(SMS_API + "_API_CLASS");
                                    TextMessageXML = await _DBC.LookupWithKey(SMS_API + "_SMS_CALLBACK_URL");
                                    FromNumber = await _DBC.LookupWithKey(SMS_API + "_FROM_NUMBER");
                                    textitem.SMSAPI = SMS_API;
                                    textitem.CoPilotID = MESSAGING_COPILOT_SID;
                                }

                                textitem.ClientId = SMSClientId; textitem.ClientSecret = SMSClientSecret; textitem.FromNumber = FromNumber;
                                textitem.APIClass = SMSAPIClass; textitem.CallBackUrl = TextMessageXML; textitem.SendInDirect = SendInDirect; textitem.TwilioRoutingApi = TwilioRoutingApi;

                            }

                            if (method.ToUpper() == "TEXT" || method.ToUpper() == "WHATSAPP")
                            {
                                INCLUDE_SENDER_IN_SMS = Convert.ToBoolean(_DBC.GetCompanyParameter("INC_SENDER_IN_SMS", item.CompanyId));
                                textitem.SendOriginalText = true; textitem.AllowPingAckByText = true;
                                textitem.AllowIncidentAckbByText = true; textitem.IncludeSenderInText = INCLUDE_SENDER_IN_SMS;
                                textitem.GenericText = "";

                                //DBC.CreateLog("INFO", "Step 11");
                            }
                            newAckOption = await _DBC.GetAckOptions(messageId);
                            OneTimeParams = true;
                        }

                        item.MessageText = item.MessageText;

                        string message = "";
                        if (method.ToUpper() == "EMAIL")
                        {
                            message = ParamsHelper.MergeEmailParams(emailitem, item);
                        }
                        else if (method.ToUpper() == "TEXT")
                        {
                            message = ParamsHelper.MergeTextParams(textitem, item);
                        }

                        var body = Encoding.UTF8.GetBytes(message);

                        routingKey = method.ToLower() + "_" + messageId;

                        routingKey = "pa_" + routingKey + "_" + QueueCount;

                        if (!RoutingKeys.Contains(routingKey))
                        {
                            model.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false);
                            model.QueueBind(queue: routingKey, exchange: exchange_name, routingKey: routingKey);

                            RoutingKeys.Add(routingKey);
                           await _DBC.MessageProcessLog(messageId, "MESSAGE_QUEUE_PUBLISHING", "PUBLIC", routingKey, "Count: " + device_list.Count);
                        }

                        model.BasicPublish(exchange: exchange_name, routingKey: routingKey, basicProperties: properties, body: body);
                    }
                    //DBC.MessageProcessLog(MessageID, "MESSAGE_QUEUE_PUBLISHED", "PUBLIC", "", "Total published: " + PublishCount);
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            await _MSG.CreateProcessQueue(messageId, "", "PUBLIC", "REQUEUE", 999);
            //MessageHelpers.Common CMN = new MessageHelpers.Common();
            //CMN.NotifyRabbitServiceFailure(ex, "Critical!!: " + "PUBLIC" + MessageID + " Queue was not published, please check the system immediatly", false);
            return false;
        }
    }
    public async Task<bool> CreateCommsLogDumpSession(string entry)
    {


        try
        {
            string RabbitMQUser = await _DBC.LookupWithKey("RABBITMQ_USER");
            string RabbitMQPassword = await _DBC.LookupWithKey("RABBITMQ_PASSWORD");
            ushort RabbitMQHeartBeat = Convert.ToUInt16(_DBC.LookupWithKey("RABBIT_HEARTBEAT_CHECK"));

            var rabbithosts = RabbitHosts(out RabbitVirtualHost);

            var factory = new ConnectionFactory()
            {
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = new TimeSpan(0, 0, 15),
                RequestedHeartbeat = TimeSpan.FromHours(RabbitMQHeartBeat),
                UserName = RabbitMQUser,
                Password = RabbitMQPassword,
                VirtualHost = RabbitVirtualHost
            };

            using (var connection = factory.CreateConnection(rabbithosts))
            using (var model = connection.CreateModel())
            {
                string exchange_name = await _DBC.LookupWithKey("RABBITMQ_QUEUE_EXCHANGE");
                model.ExchangeDeclare(exchange: exchange_name, type: "direct", durable: true, autoDelete: false);

                IBasicProperties properties = model.CreateBasicProperties();
                properties.Persistent = true;
                properties.DeliveryMode = 2;

                string routingKey = "comms_log_dump_sessions";
                try
                {
                    model.QueueDeclare(queue: routingKey, durable: true, exclusive: false, autoDelete: false);
                    model.QueueBind(queue: routingKey, exchange: exchange_name, routingKey: routingKey);

                    var body = Encoding.UTF8.GetBytes(entry);

                    model.BasicPublish(exchange: exchange_name, routingKey: routingKey, basicProperties: properties, body: body);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return true;
        }
        catch (Exception ex)
        {

            return false;
        }
    }
}