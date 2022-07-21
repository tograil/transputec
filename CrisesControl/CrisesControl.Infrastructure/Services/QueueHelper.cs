using CrisesControl.Api.Application.Helpers;
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

namespace CrisesControl.Infrastructure.Services
{
    public static class QueueHelper
    {
        static string ServiceHost = string.Empty;
        public static string RabbitVirtualHost = "/";
        public static CrisesControlContext db;
        private static IHttpContextAccessor _httpContextAccessor;
        public static void MessageDeviceQueue(int MessageID, string MessageType, int Priority, int CascadePlanID = 0)
        {
            DBCommon DBC = new DBCommon(db,_httpContextAccessor);
            try
            {
               
                    var pMessageId = new SqlParameter("@MessageID", MessageID);
                    var pMessageType = new SqlParameter("@MessageType", MessageType);
                    var pPriority = new SqlParameter("@Priority", Priority);
                    db.Database.GetCommandTimeout();

                    string sp_name = "Pro_Create_Message_Queue ";

                    if (CascadePlanID > 0)
                        sp_name = "Pro_Create_Message_Queue_Cascading ";

                     DBC.LocalException(MessageID.ToString(), sp_name, Priority.ToString());

                    var List = db.Database.ExecuteSqlRaw(sp_name + " @MessageID, @MessageType, @Priority", pMessageId, pMessageType, pPriority);

                    Task.Factory.StartNew(() => MessageDevicePublish(MessageID, 1, ""));
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void MessageDevicePublish(int MessageID, int Priority, string Method = "")
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                var rabbithosts = RabbitHosts(out RabbitVirtualHost);
                List<Task> queuetask = new List<Task>();
                //Task temail = null; Task tpush = null; Task tphone = null; Task ttext = null; Task twhatsapp = null;

                if (Method == "" || Method == "EMAIL")
                {
                    //Console.WriteLine("Publishing messages for " + Method);
                    var temail = Task.Factory.StartNew(() => PublishMessageQueue(MessageID, rabbithosts, "EMAIL", null, Priority));
                    if (temail != null)
                        queuetask.Add(temail);
                }


                if (Method == "" || Method == "PUSH")
                {
                    //Console.WriteLine("Publishing messages for " + Method);
                    var tpush = Task.Factory.StartNew(() => PublishMessageQueue(MessageID, rabbithosts, "PUSH", null, Priority));
                    if (tpush != null)
                        queuetask.Add(tpush);
                }


                if (Method == "" || Method == "PHONE")
                {
                    //Console.WriteLine("Publishing messages for " + Method);
                    var tphone = Task.Factory.StartNew(() => PublishMessageQueue(MessageID, rabbithosts, "PHONE", null, Priority));
                    if (tphone != null)
                        queuetask.Add(tphone);
                }


                if (Method == "" || Method == "TEXT")
                {
                    //Console.WriteLine("Publishing messages for " + Method);
                    var ttext = Task.Factory.StartNew(() => PublishMessageQueue(MessageID, rabbithosts, "TEXT", null, Priority));
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
        public static async Task<bool> PublishMessageQueue(int MessageID, List<string> RabbitHost, string Method,
            List<MessageQueueItem> devicelist = null, int Priority = 1)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            Messaging MSG = new Messaging(db,_httpContextAccessor);

            try
            {
                await  MSG.CreateProcessQueue(MessageID, "", Method, "CONFIRM", 99);

                string RabbitMQUser = DBC.LookupWithKey("RABBITMQ_USER");
                string RabbitMQPassword = DBC.LookupWithKey("RABBITMQ_PASSWORD");

                ushort RabbitMQHeartBeat = Convert.ToUInt16(DBC.LookupWithKey("RABBIT_HEARTBEAT_CHECK"));
                var factory = new ConnectionFactory()
                {
                    AutomaticRecoveryEnabled = true,
                    TopologyRecoveryEnabled = true,
                    NetworkRecoveryInterval = new TimeSpan(0, 0, 15),
                    RequestedHeartbeat = TimeSpan.FromTicks(RabbitMQHeartBeat),
                    UserName = RabbitMQUser,
                    Password = RabbitMQPassword,
                    VirtualHost = RabbitVirtualHost
                };

                using (var connection = factory.CreateConnection(RabbitHost))
                using (var model = connection.CreateModel())
                {
                    string exchange_name = DBC.LookupWithKey("RABBITMQ_QUEUE_EXCHANGE");
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
                        device_list = GetDeviceQueue(MessageID, Method, 0, Priority);
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

                            bool.TryParse(DBC.LookupWithKey("COMMS_DEBUG_MODE"), out CommsDebug);
                            SendInDirect = DBC.IsTrue(DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                            MAXMSGATTEMPT = Convert.ToInt32(DBC.LookupWithKey("MAXMSGATTEMPT"));
                            double MaxQueueNumber = Convert.ToDouble(DBC.LookupWithKey("RABBIT_QUEUE_SIZE"));
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

                            TwilioRoutingApi = DBC.LookupWithKey("TWILIO_ROUTING_API");

                            if (Method.ToUpper() == "EMAIL")
                            {
                                emailitem = ParamsHelper.GetEmailParams();
                            }
                            else if (Method.ToUpper() == "PUSH")
                            {
                                pushitem = ParamsHelper.GetPushParams();
                            }
                            else if (Method.ToUpper() == "TEXT")
                            {
                                textitem = ParamsHelper.GetTextParams();
                            }
                            else if (Method.ToUpper() == "PHONE")
                            {
                                phoneitem = ParamsHelper.GetPhoneParams();
                            }
                            else if (Method.ToUpper() == "WHATSAPP")
                            {
                                waitem = ParamsHelper.GetWhatsAppParams();
                            }
                      

                        List<string> RoutingKeys = new List<string>();
                        string routingKey = "";
                        string push_type = "";

                        int PublishCount = 0;
                        int QueueItemCount = 0;
                        int QueueCount = 0;
                        string CurrentPushType = "";
                        string tmproutingKey = "";

                        if (Method.ToUpper() != "PUSH")
                        {
                            MessageQueueItem tmpitem = new MessageQueueItem();
                            tmpitem.Status = "SKIP";
                            string tmpmessage = JsonConvert.SerializeObject(tmpitem);
                            var tmpbody = Encoding.UTF8.GetBytes(tmpmessage);
                            tmproutingKey = Method.ToLower() + "_" + QueueCount;

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
                                && (Method.ToUpper() == "PHONE" || Method.ToUpper() == "TEXT"))
                            {
                                continue;
                            }

                            //if((Method.ToUpper() == "TEXT" || Method.ToUpper() == "PHONE" || Method.ToUpper() == "WHATSAPP") && item.MobileNo.Length < 4) {
                            //    continue;
                            //}

                            if (!OneTimeParams)
                            {
                                SIMULATION_TEXT = DBC.GetCompanyParameter("INCIDENT_SIMULATION_TEXT", item.CompanyId);
                                PHONE_SIMULATION_MESSAGE = DBC.GetCompanyParameter("INCIDENT_SIMULATION_PHONE", item.CompanyId);


                                if (Method.ToUpper() == "EMAIL")
                                {
                                    OneClickAck = DBC.GetCompanyParameter("ONE_CLICK_EMAIL_ACKNOWLEDGE", item.CompanyId);
                                    EmailProvider = DBC.GetCompanyParameter("EMAIL_PROVIDER", item.CompanyId);
                                    emailitem.OneClickAcknowledge = OneClickAck;
                                    emailitem.EmailProvider = EmailProvider;

                                }
                                else if (Method.ToUpper() == "PHONE" || Method.ToUpper() == "TEXT")
                                {
                                    if (Method.ToUpper() == "PHONE")
                                    {
                                        string VOICE_API = DBC.GetCompanyParameter("VOICE_API", item.CompanyId);
                                        if (VOICE_API == "UNIFONIC")
                                        {
                                            VoiceClientId = DBC.GetCompanyParameter(VOICE_API + "_PHONE_CLIENTID", item.CompanyId);
                                        }
                                        else
                                        {
                                            VoiceClientId = DBC.LookupWithKey(VOICE_API + "_CLIENTID");
                                        }

                                        VoiceClientSecret = DBC.LookupWithKey(VOICE_API + "_CLIENT_SECRET");
                                        FromNumber = DBC.LookupWithKey(VOICE_API + "_FROM_NUMBER");
                                        CallBackUrl = DBC.LookupWithKey(VOICE_API + "_CALLBACK_URL");
                                        VoiceAPIClass = DBC.LookupWithKey(VOICE_API + "_API_CLASS");
                                        MessageXML = DBC.LookupWithKey(VOICE_API + "_MESSAGE_XML_URL");
                                        RetryNumberList = DBC.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST", item.CompanyId, FromNumber);
                                        FromNumberList = RetryNumberList.Split(',').ToList();
                                        phoneitem.VoiceAPI = VOICE_API;
                                    }

                                    if (Method.ToUpper() == "TEXT")
                                    {
                                        string SMS_API = DBC.GetCompanyParameter("SMS_API", item.CompanyId);
                                        string MESSAGING_COPILOT_SID = DBC.GetCompanyParameter("MESSAGING_COPILOT_SID", item.CompanyId);
                                        if (SMS_API == "UNIFONIC")
                                        {
                                            SMSClientId = DBC.GetCompanyParameter(SMS_API + "_CLIENTID", item.CompanyId);
                                            SMSClientSecret = DBC.GetCompanyParameter(SMS_API + "_CLIENT_SECRET", item.CompanyId);
                                        }
                                        else
                                        {
                                            SMSClientId = DBC.LookupWithKey(SMS_API + "_CLIENTID");
                                            SMSClientSecret = DBC.LookupWithKey(SMS_API + "_CLIENT_SECRET");
                                        }

                                        SMSAPIClass = DBC.LookupWithKey(SMS_API + "_API_CLASS");
                                        TextMessageXML = DBC.LookupWithKey(SMS_API + "_SMS_CALLBACK_URL");
                                        FromNumber = DBC.LookupWithKey(SMS_API + "_FROM_NUMBER");
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

                                if (Method.ToUpper() == "TEXT" || Method.ToUpper() == "WHATSAPP")
                                {
                                    SEND_ORIGINAL_TEXT = Convert.ToBoolean(DBC.GetCompanyParameter("SEND_ORIGINAL_TEXT", item.CompanyId));
                                    ReplyToNumber = DBC.GetCompanyParameter("REPLY_TO_NUMBER", item.CompanyId);

                                    GENERIC_TEXT = DBC.GetCompanyParameter("MESSAGE_TEXT_GENERIC", item.CompanyId);

                                    if (Method.ToUpper() == "WHATSAPP")
                                    {
                                        AllowPingAckByWA = Convert.ToBoolean(DBC.GetCompanyParameter("SEND_ORIGINAL_WATEXT", item.CompanyId)); ;
                                        AllowIncidentAckbByWA = Convert.ToBoolean(DBC.GetCompanyParameter("SEND_ORIGINAL_WATEXT", item.CompanyId)); ;
                                        IncludeSenderInWA = Convert.ToBoolean(DBC.GetCompanyParameter("SEND_ORIGINAL_WATEXT", item.CompanyId)); ;
                                        waitem.AllowIncidentAckbByWA = AllowIncidentAckbByWA; waitem.AllowPingAckByWA = AllowPingAckByWA;
                                        waitem.IncludeSenderInWA = IncludeSenderInWA;
                                    }
                                    else
                                    {
                                        ALLOW_ACKNOWLEDGE_PING = Convert.ToBoolean(DBC.GetCompanyParameter("ALLOW_PING_ACK_BY_TEXT", item.CompanyId));
                                        ALLOW_ACKNOWLEDGE_INCIDENT = Convert.ToBoolean(DBC.GetCompanyParameter("ALLOW_INCI_ACK_BY_TEXT", item.CompanyId));
                                        INCLUDE_SENDER_IN_SMS = Convert.ToBoolean(DBC.GetCompanyParameter("INC_SENDER_IN_SMS", item.CompanyId));
                                        textitem.SendOriginalText = SEND_ORIGINAL_TEXT; textitem.AllowPingAckByText = ALLOW_ACKNOWLEDGE_PING;
                                        textitem.AllowIncidentAckbByText = ALLOW_ACKNOWLEDGE_INCIDENT; textitem.IncludeSenderInText = INCLUDE_SENDER_IN_SMS;
                                        textitem.GenericText = GENERIC_TEXT; textitem.ReplyToNumber = ReplyToNumber;
                                    }
                                }
                                newAckOption = await DBC.GetAckOptions(MessageID);
                                OneTimeParams = true;
                            }
                            else if (Method.ToUpper() == "PUSH")
                            {
                                int.TryParse(DBC.GetCompanyParameter("TRACKING_DURATION", item.CompanyId), out TrackingDuration);
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


                            if (item.TransportType.ToUpper() == "SIMULATION" && Method.ToUpper() == "PHONE")
                            {
                                item.MessageText = PHONE_SIMULATION_MESSAGE + " " + item.MessageText;
                            }
                            else if (item.TransportType.ToUpper() == "SIMULATION" && Method.ToUpper() != "PHONE")
                            {
                                item.MessageText = SIMULATION_TEXT + " " + item.MessageText;
                            }
                            else
                            {
                                item.MessageText = item.MessageText;
                            }


                            string message = "";
                            if (Method.ToUpper() == "EMAIL")
                            {
                                message = ParamsHelper.MergeEmailParams(emailitem, item);
                            }
                            else if (Method.ToUpper() == "PHONE")
                            {
                                phoneitem.FromNumber = DBC.GetValueByIndex(FromNumberList, item.Attempt);
                                message = ParamsHelper.MergePhoneParams(phoneitem, item);
                            }
                            else if (Method.ToUpper() == "TEXT")
                            {
                                message = ParamsHelper.MergeTextParams(textitem, item);
                            }
                            else if (Method.ToUpper() == "PUSH")
                            {
                                pushitem.AckOptions = newAckOption;
                                message = ParamsHelper.MergePushParams(pushitem, item);
                            }
                            else if (Method.ToUpper() == "WHATSAPP")
                            {
                                message = ParamsHelper.MergeWAParams(waitem, item);
                            }


                            var body = Encoding.UTF8.GetBytes(message);

                            if (Method.ToUpper() == "PUSH")
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
                                routingKey = Method.ToLower();
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

                                DBC.MessageProcessLog(MessageID, "MESSAGE_QUEUE_PUBLISHING", Method, routingKey, "Count: " + QueueItemCount);

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
                        DBC.MessageProcessLog(MessageID, "MESSAGE_QUEUE_PUBLISHED", Method, "", "Total published: " + PublishCount);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                
                await MSG.CreateProcessQueue(MessageID, "", Method, "REQUEUE", 999);
               // MessageHelpers.Common CMN = new MessageHelpers.Common();
               // CMN.NotifyRabbitServiceFailure(ex, "Critical!!: " + Method + MessageID + " Queue was not published, please check the system immediatly", false);
                return false;
            }
        }
        public static List<string> RabbitHosts(out string VirtualHost)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            string RabbitHost = DBC.LookupWithKey("RABBITMQ_HOST");
            VirtualHost = DBC.LookupWithKey("RABBITMQ_VIRTUAL_HOST");

            List<string> hostlist = RabbitHost.Split(',').ToList();
            return hostlist;
        }
        public static List<MessageQueueItem> GetDeviceQueue(int MessageID, string Method, int MessageDeviceId = 0, int Priority = 1)
        {
            DBCommon DBC = new DBCommon(db, _httpContextAccessor);
            try
            {
                
                    var pMessageId = new SqlParameter("@MessageID", MessageID);
                    var pMethod = new SqlParameter("@Method", Method);
                    var pMessageDeviceId = new SqlParameter("@MessageDeviceID", MessageDeviceId);
                    var pPriority = new SqlParameter("@Priority", Priority);


                    db.Database.GetCommandTimeout();
                    var List = db.Set<MessageQueueItem>().FromSqlRaw("Pro_Get_Message_Device_Queue @MessageID,@Method,@MessageDeviceID,@Priority",
                        pMessageId, pMethod, pMessageDeviceId, pPriority).ToList().Select(c => {
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


    }
}
