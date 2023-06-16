
using CrisesControl.Core.Models;
using CrisesControl.Core.Queues;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using CrisesControl.SharedKernel.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Infrastructure.Repositories;

namespace CrisesControl.Infrastructure.Services {
    public static class ParamsHelper {
        private static CrisesControlContext _context;
        private static readonly IHttpContextAccessor _httpContextAccessor;
        private static IDBCommonRepository _DBC;
        //private static readonly ISenderEmailService _SDE;
        static ParamsHelper() {
            _httpContextAccessor = new HttpContextAccessor();
            // DBC = new DBCommonRepository(_context,_httpContextAccessor, _SDE);
        }

        public static async Task InitializeParamsHelper(IDBCommonRepository DBC) {
            _DBC = DBC;
        }

        public static async Task<EmailMessage> GetEmailParams() {


            EmailMessage item = new EmailMessage();
            try {
                string TwiterLink = "", FacebookLink = "", LinkedinLink = "", Domain = "", TwiterIcon = "", FacebookIcon = "",
                            LinkedinIcon = "", UsersupportLink = "", CCimage = "", Portal = "", AckURL = "",
                            MyTaskURL = "", TaskURLLabel = "", Templatepath = "", EmailFrom = "", SMTPHost = "", EmailSub = "", SendGridAPIKey = "",
                            AwsSesAccessKey = "", AWSSESSecretKey = "", Office365Host = "";



                TwiterLink = await _DBC.LookupWithKey("CC_TWITTER_PAGE");
                TwiterIcon = await _DBC.LookupWithKey("CC_TWITTER_ICON");
                FacebookLink = await _DBC.LookupWithKey("CC_FB_PAGE");
                FacebookIcon = await _DBC.LookupWithKey("CC_FB_ICON");
                LinkedinLink = await _DBC.LookupWithKey("CC_LINKEDIN_PAGE");
                LinkedinIcon = await _DBC.LookupWithKey("CC_LINKEDIN_ICON");
                Domain = await _DBC.LookupWithKey("DOMAIN");
                Portal = await _DBC.LookupWithKey("PORTAL");
                AckURL = await _DBC.LookupWithKey("ACKNOWLEDGE_URL");
                UsersupportLink = await _DBC.LookupWithKey("CC_USER_SUPPORT_LINK");
                CCimage = await _DBC.LookupWithKey("CCLOGO");
                Templatepath = await _DBC.LookupWithKey("COMMS_TEMPLATE_PATH");
                EmailFrom = await _DBC.LookupWithKey("ALERT_EMAILFROM");
                SMTPHost = await _DBC.LookupWithKey("SMTPHOST");
                EmailSub = await _DBC.LookupWithKey("EMAILSUB");
                MyTaskURL = await _DBC.LookupWithKey("INCIDENT_TASK_URL");
                TaskURLLabel = await _DBC.LookupWithKey("TASK_URL_LABEL");
                SendGridAPIKey = await _DBC.LookupWithKey("SEND_GRID_API_KEY");
                AwsSesAccessKey = await _DBC.LookupWithKey("AWS_SES_ACCCESS_KEY");
                AWSSESSecretKey = await _DBC.LookupWithKey("AWS_SES_SECRET_KEY");
                Office365Host = await _DBC.LookupWithKey("OFFICE365_HOST");

                item.TwitterIcon = TwiterIcon; item.FacebookIcon = FacebookIcon; item.LinkedInIcon = LinkedinIcon; item.Domain = Domain;
                item.TwitterPage = TwiterLink; item.FacebookPage = FacebookLink; item.LinkedInPage = LinkedinLink; item.SupportEmail = UsersupportLink;
                item.CCLogo = CCimage; item.PortalURL = Portal; item.ACKUrl = AckURL; item.TemplatePath = Templatepath; item.EmailFrom = EmailFrom;
                item.SMTPHost = SMTPHost; item.EmailSub = EmailSub; item.TaskURL = MyTaskURL; item.TaskURLLabel = TaskURLLabel; item.SendGridAPIKey = SendGridAPIKey;
                item.AwsSesAccessKey = AwsSesAccessKey; item.AWSSESSecretKey = AWSSESSecretKey; item.Office365Host = Office365Host;

                return item;
            } catch (System.Exception) {
                return item;
            }
        }
        //public static string LookupWithKey(string Key, string Default = "") {
        //    try {
        //        CrisesControlContext _context = null;
        //        Dictionary<string, string> Globals = CCConstants.GlobalVars;
        //        if (Globals.ContainsKey(Key)) {
        //            return Globals[Key];
        //        }


        //        var LKP = _context.Set<SysParameter>()
        //                   .Where(L => L.Name == Key
        //                   ).FirstOrDefault();
        //        if (LKP != null) {
        //            Default = LKP.Value;
        //        }

        //        return Default;
        //    } catch (Exception ex) {
        //        throw ex;
        //        return Default;
        //    }
        //}

        public static async Task<PushMessage> GetPushParams() {
            PushMessage item = new PushMessage();
            try {
                bool APPLEPUSHMODE = false;
                string APPLECERPWD = "", APPLECERTPATH = "", GOOGLE_API_KEY = "", SOUNDFILE = "", BB_APPLICATION_ID = "",
                            BB_PASSOWRD = "", BBPUSHURL = "", WINAPP_PACKAGE_NAME = "", WINAPP_SID = "", WINAPP_CLIENT_SECRET = "",
                            WINDESK_PACKAGE_NAME = "", WINDESK_SID = "", WINDESK_CLIENT_SECRET = "";




                APPLECERPWD = await _DBC.LookupWithKey("APPLECERPWD");
                APPLECERTPATH = await _DBC.LookupWithKey("APPLECERTPATH");
                APPLEPUSHMODE = Convert.ToBoolean(await _DBC.LookupWithKey("APPLEPUSHMODE"));
                GOOGLE_API_KEY = await _DBC.LookupWithKey("GOOGLE_API_KEY");
                SOUNDFILE = await _DBC.LookupWithKey("PUSH_SOUND_FILE");
                WINAPP_PACKAGE_NAME = await _DBC.LookupWithKey("WINAPP_PACKAGE_NAME");
                WINAPP_SID = await _DBC.LookupWithKey("WINAPP_SID");
                WINAPP_CLIENT_SECRET = await _DBC.LookupWithKey("WINAPP_CLIENT_SECRET");
                WINDESK_PACKAGE_NAME = await _DBC.LookupWithKey("WINDESK_PACKAGE_NAME");
                WINDESK_SID = await _DBC.LookupWithKey("WINDESK_SID");
                WINDESK_CLIENT_SECRET = await _DBC.LookupWithKey("WINDESK_CLIENT_SECRET");

                item.AppleCertPath = APPLECERTPATH; item.AppleCertPwd = APPLECERPWD; item.ApplePushMode = APPLEPUSHMODE;
                item.BBApplicationID = BB_APPLICATION_ID; item.BBPassword = BB_PASSOWRD; item.BBPushURL = BBPUSHURL;
                item.GoogleApiKey = GOOGLE_API_KEY; item.SoundFileName = SOUNDFILE;
                item.WinAppSID = WINAPP_SID; item.WinPackageName = WINAPP_PACKAGE_NAME; item.WinClientSecret = WINAPP_CLIENT_SECRET;
                item.WinDeskSID = WINDESK_SID; item.WinDeskPackageName = WINDESK_PACKAGE_NAME; item.WinDeskClientSecret = WINDESK_CLIENT_SECRET;
                item.PortalUrl = await _DBC.LookupWithKey("PORTAL");

                return item;
            } catch (System.Exception) {
                return item;
            }
        }

        public static async Task<TextMessage> GetTextParams() {
            TextMessage item = new TextMessage();
            try {
                bool USE_MESSAGING_COPILOT = false;
                //MESSAGING_COPILOT_SID = "",
                string MyTaskURL = "", TaskURLLabel = "";



                MyTaskURL = await _DBC.LookupWithKey("INCIDENT_TASK_URL");
                TaskURLLabel = await _DBC.LookupWithKey("TASK_URL_LABEL");

                USE_MESSAGING_COPILOT = Convert.ToBoolean(await _DBC.LookupWithKey("USE_MESSAGING_COPILOT"));
                //MESSAGING_COPILOT_SID = DBC.LookupWithKey("MESSAGING_COPILOT_SID");

                //item.CoPilotID = MESSAGING_COPILOT_SID; 
                item.UseCopilot = USE_MESSAGING_COPILOT;
                item.TaskURL = MyTaskURL; item.TaskURLLabel = TaskURLLabel;

                return item;
            } catch (System.Exception) {
                return item;
            }
        }

        public static async Task<TextMessage> GetWhatsAppParams() {
            TextMessage item = new TextMessage();
            try {

                string WAInComingApi = "", WAStatusCallback = "", MyTaskURL = "", TaskURLLabel = "";


                MyTaskURL = await _DBC.LookupWithKey("INCIDENT_TASK_URL");
                TaskURLLabel = await _DBC.LookupWithKey("TASK_URL_LABEL");

                WAInComingApi = await _DBC.LookupWithKey("WHATSAPP_INCOMING_API");
                WAStatusCallback = await _DBC.LookupWithKey("WHATSAPP_STATUS_CALLBACK");

                item.WAInComingApi = WAInComingApi; item.WAStatusCallback = WAStatusCallback;
                item.TaskURL = MyTaskURL; item.TaskURLLabel = TaskURLLabel;

                return item;
            } catch (System.Exception) {
                return item;
            }
        }

        public static PhoneMessage GetPhoneParams() {
            PhoneMessage item = new PhoneMessage();
            try {
                return item;
            } catch (System.Exception) {
                return item;
            }
        }

        public static string MergeEmailParams(EmailMessage item, MessageQueueItem mqitem) {
            string message = string.Empty;

            item.MessageDeviceId = mqitem.MessageDeviceId;
            item.CommsDebug = mqitem.CommsDebug;
            item.Status = mqitem.Status;
            item.Attempt = mqitem.Attempt;
            item.FirstName = mqitem.FirstName;
            item.LastName = mqitem.LastName;
            item.MessageType = mqitem.MessageType;
            item.IncidentActivationId = mqitem.IncidentActivationId;
            item.CreatedBy = mqitem.CreatedBy;
            item.MessageText = mqitem.MessageText;
            item.IsTaskRecepient = mqitem.IsTaskRecepient;
            item.CompanyId = mqitem.CompanyId;
            item.CompanyLogoPath = mqitem.CompanyLogoPath;
            item.MessageListId = mqitem.MessageListId;
            item.Company_Name = mqitem.Company_Name;
            item.DeviceAddress = mqitem.UserEmail;
            item.UserId = mqitem.UserId;
            item.MaxAttempt = mqitem.MaxAttempt;
            item.MessageId = mqitem.MessageId;
            item.Method = mqitem.Method;
            item.SenderName = mqitem.SenderName;
            item.CreatedTimeZone = mqitem.CreatedTimeZone;
            item.UserEmail = mqitem.UserEmail;
            item.ParentID = mqitem.ParentID;
            item.CustomerId = mqitem.CustomerId;
            item.MessageActionType = mqitem.MessageActionType;
            item.MessageSourceAction = mqitem.MessageSourceAction;

            message = JsonConvert.SerializeObject(item);
            return message;
        }

        public static string MergePhoneParams(PhoneMessage item, MessageQueueItem mqitem) {
            string message = string.Empty;

            item.MessageDeviceId = mqitem.MessageDeviceId;
            item.CommsDebug = mqitem.CommsDebug;
            item.Status = mqitem.Status;
            item.Attempt = mqitem.Attempt;
            item.UserId = mqitem.UserId;
            item.DeviceAddress = CrisesControl.SharedKernel.Utils.StringExtensions.FormatMobile(mqitem.ISDCode, mqitem.MobileNo);
            item.MessageId = mqitem.MessageId;
            item.MessageListId = mqitem.MessageListId;
            item.Method = mqitem.Method;
            item.MessageText = mqitem.MessageText;
            item.MaxAttempt = mqitem.MaxAttempt;
            item.SenderName = mqitem.SenderName;
            item.CreatedTimeZone = mqitem.CreatedTimeZone;
            item.ParentID = mqitem.ParentID;
            item.MessageActionType = mqitem.MessageActionType;
            item.MessageSourceAction = mqitem.MessageSourceAction;

            message = JsonConvert.SerializeObject(item);
            return message;
        }

        public static string MergeTextParams(TextMessage item, MessageQueueItem mqitem) {
            string message = string.Empty;

            item.MessageDeviceId = mqitem.MessageDeviceId;
            item.CommsDebug = mqitem.CommsDebug;
            item.Status = mqitem.Status;
            item.Attempt = mqitem.Attempt;
            item.UserId = mqitem.UserId;
            item.ISDCode = mqitem.ISDCode;
            item.DeviceAddress = CrisesControl.SharedKernel.Utils.StringExtensions.FormatMobile(mqitem.ISDCode, mqitem.MobileNo);
            item.MessageId = mqitem.MessageId;
            item.MessageListId = mqitem.MessageListId;
            item.Method = mqitem.Method;
            item.MessageText = mqitem.MessageText;
            item.MaxAttempt = mqitem.MaxAttempt;
            item.Company_Name = mqitem.Company_Name;
            item.MessageType = mqitem.MessageType;
            item.CreatedTimeZone = mqitem.CreatedTimeZone;
            item.CompanyId = mqitem.CompanyId;
            item.MultiResponse = mqitem.MultiResponse;
            item.SenderName = mqitem.SenderName;
            item.ParentID = mqitem.ParentID;
            item.MessageActionType = mqitem.MessageActionType;
            item.MessageSourceAction = mqitem.MessageSourceAction;

            message = JsonConvert.SerializeObject(item);
            return message;
        }

        public static string MergeWAParams(TextMessage item, MessageQueueItem mqitem) {
            string message = string.Empty;
            item.MessageDeviceId = mqitem.MessageDeviceId;
            item.CommsDebug = mqitem.CommsDebug;
            item.Status = mqitem.Status;
            item.Attempt = mqitem.Attempt;
            item.UserId = mqitem.UserId;
            item.ISDCode = mqitem.ISDCode;
            item.DeviceAddress = CrisesControl.SharedKernel.Utils.StringExtensions.FormatMobile(mqitem.ISDCode, mqitem.MobileNo);
            item.MessageId = mqitem.MessageId;
            item.MessageListId = mqitem.MessageListId;
            item.Method = mqitem.Method;
            item.MessageText = mqitem.MessageText;
            item.MaxAttempt = mqitem.MaxAttempt;
            item.Company_Name = mqitem.Company_Name;
            item.MessageType = mqitem.MessageType;
            item.CreatedTimeZone = mqitem.CreatedTimeZone;
            item.CompanyId = mqitem.CompanyId;
            item.MultiResponse = mqitem.MultiResponse;
            item.SenderName = mqitem.SenderName;
            item.ParentID = mqitem.ParentID;
            item.MessageActionType = mqitem.MessageActionType;
            item.MessageSourceAction = mqitem.MessageSourceAction;

            message = JsonConvert.SerializeObject(item);
            return message;
        }

        public static string MergePushParams(PushMessage item, MessageQueueItem mqitem) {
            string message = string.Empty;

            item.MessageDeviceId = mqitem.MessageDeviceId;
            item.CommsDebug = mqitem.CommsDebug;
            item.Status = mqitem.Status;
            item.Attempt = mqitem.Attempt;
            item.DeviceType = mqitem.DeviceType;
            item.MessageType = mqitem.MessageType;
            item.IncidentActivationId = mqitem.IncidentActivationId;
            item.MessageId = mqitem.MessageId;
            item.Company_Name = mqitem.Company_Name;
            item.UserId = mqitem.UserId;
            item.CompanyId = mqitem.CompanyId;
            item.TrackUser = mqitem.TrackUser;
            item.SirenON = mqitem.SirenON;
            item.OverrideSilent = mqitem.OverrideSilent;
            item.SoundFile = mqitem.SoundFile;
            item.SilentMessage = mqitem.SilentMessage;
            item.DeviceAddress = mqitem.DeviceAddress;
            item.MessageText = mqitem.MessageText;
            item.MessageListId = mqitem.MessageListId;
            item.Method = mqitem.Method;
            item.MaxAttempt = mqitem.MaxAttempt;
            item.SenderName = mqitem.SenderName;
            item.CreatedTimeZone = mqitem.CreatedTimeZone;
            item.ParentID = mqitem.ParentID;
            item.CustomerId = mqitem.CustomerId;
            item.DateSent = mqitem.DateSent;
            item.BadgeCount = mqitem.BadgeCount;
            item.MessageActionType = mqitem.MessageActionType;
            item.MessageSourceAction = mqitem.MessageSourceAction;

            message = JsonConvert.SerializeObject(item);
            return message;
        }
    }
}
