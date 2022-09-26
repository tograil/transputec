using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace CrisesControl.Infrastructure.Services
{
    public class CommsHelper
    {
        private readonly DBCommon _DBC;
        private readonly CrisesControlContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Messaging _MSG;
        private readonly SendEmail _SDE;

        public int GCompanyId;
        public int GUserId;
        public string GTimezoneId = "GMT Standard Time";

        public CommsHelper( CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
           
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _DBC = new DBCommon(_context, _httpContextAccessor);
            _MSG = new Messaging(_context,_httpContextAccessor,_DBC);
            _SDE = new SendEmail(_context, _DBC); ;
        }

        public async Task<string> StartConference(int companyId, int userId, List<int> userList, string timeZoneId, int activeIncidentId = 0, int messageId = 0, string objectType = "Incident")
        {

            string ClientId = string.Empty;
            string ClientSecret = string.Empty;
            string FromNumber = string.Empty;
            string APIClass = string.Empty;
            string CallBackUrl = string.Empty;
            string MessageXML = string.Empty;
            string TwilioRoutingApi = string.Empty;

            try
            {

                //Get the selected conferance api for the company and set the requrest api params.
                string CONF_API = _DBC.GetCompanyParameter("CONFERANCE_API", companyId);
                bool RecordConf = Convert.ToBoolean(_DBC.GetCompanyParameter("RECORD_CONFERENCE", companyId));
                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                dynamic CommsAPI = this.InitComms(CONF_API);
                CommsAPI.IsConf = true;
                CommsAPI.ConfRoom = "ConfRoom_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                FromNumber = _DBC.LookupWithKey(CONF_API + "_FROM_NUMBER");
                CallBackUrl = _DBC.LookupWithKey(CONF_API + "_CONF_STATUS_CALLBACK_URL");
                MessageXML = _DBC.LookupWithKey(CONF_API + "_CONF_XML_URL");

                int confMobOrder = Convert.ToInt16(_DBC.LookupWithKey("CONF_MOBILE_ORDER"));
                int confgLLOrder = Convert.ToInt16(_DBC.LookupWithKey("CONF_LANDLINE_ORDER"));

                //Get the user list to fetch their mobile numbers
                var tmpUserList = (from U in _context.Set<User>() where U.CompanyId == companyId && userList.Contains(U.UserId) && U.Status == 1 select U).Distinct().ToList();
                var mobileList = (from mU in tmpUserList select new { UserId = mU.UserId, ISD = mU.Isdcode, PhoneNumber = mU.MobileNo, Order = confMobOrder });
                var landlineList = (from mU in tmpUserList select new { UserId = mU.UserId, ISD = mU.Llisdcode, PhoneNumber = mU.Landline, Order = confgLLOrder });

                var FinalPhoneList = mobileList.Union(landlineList).OrderBy(o => o.UserId).ThenBy(o => o.Order).ToList();

                int ConfHeaderId = _MSG.CreateConferenceHeader(companyId, userId, timeZoneId, CommsAPI.ConfRoom, RecordConf, activeIncidentId, messageId, objectType);

                string CallId = string.Empty;

                int SkipUserId = 0;

                //Loop through with each user and their phone number to make the call
                foreach (var uItem in FinalPhoneList)
                {
                    string Status = string.Empty;

                    string toMobile = _DBC.FormatMobile(uItem.ISD, uItem.PhoneNumber);

                    if (!string.IsNullOrEmpty(uItem.PhoneNumber))
                    {
                        if (SkipUserId == 0 || SkipUserId != uItem.UserId)
                        {
                            Task<dynamic> trhtask = Task.Factory.StartNew(() => CommsAPI.Call(FromNumber, toMobile, MessageXML, CallBackUrl));
                            CommsStatus callrslt = trhtask.Result;

                            CallId = callrslt.CommsId;

                            if (!callrslt.Status)
                            {
                                //Failed call
                                SkipUserId = 0;
                                Status = CallId = _DBC.Left(CallId, 50);
                            }
                            else
                            {
                                //Success the call
                                SkipUserId = uItem.UserId;
                                Status = callrslt.CurrentAction;
                            }
                            int ConfDetailId = await _MSG.CreateConferenceDetail("ADD", ConfHeaderId, uItem.UserId, toMobile, "", CallId.ToString(), Status, timeZoneId, 0);
                        }
                        else
                        {
                            SkipUserId = 0;
                        }
                    }
                    else
                    {
                        SkipUserId = 0;
                    }
                }


            }
            catch (Exception ex)
            {
            }
            return "";
        }

        public async Task<string> StartConferenceNew(int companyId, int userId, List<User> userList, string timeZoneId, int activeIncidentId = 0, int messageId = 0, string objectType = "Incident")
        {

            //Initiatize the variables
            string ClientId = string.Empty;
            string ClientSecret = string.Empty;
            string FromNumber = string.Empty;
            string APIClass = string.Empty;
            string CallBackUrl = string.Empty;
            string MessageXML = string.Empty;
            string TwilioRoutingApi = string.Empty;

            try
            {

                //Get the selected conferance api for the company and set the requrest api params.
                string CONF_API = _DBC.GetCompanyParameter("CONFERANCE_API", companyId);
                bool RecordConf = Convert.ToBoolean(_DBC.GetCompanyParameter("RECORD_CONFERENCE", companyId));
                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                //Create instance of CommsApi choosen by company
                dynamic CommsAPI = this.InitComms(CONF_API);

                CommsAPI.IsConf = true;
                CommsAPI.ConfRoom = "ConfRoom_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                //FromNumber = DBC.LookupWithKey(CONF_API + "_FROM_NUMBER");
                //Get API configraiton from sysparameters
                string RetryNumberList = _DBC.GetCompanyParameter("PHONE_RETRY_NUMBER_LIST", companyId, FromNumber);
                List<string> FromNumberList = RetryNumberList.Split(',').ToList();

                FromNumber = FromNumberList.FirstOrDefault();
                CallBackUrl = _DBC.LookupWithKey(CONF_API + "_CONF_STATUS_CALLBACK_URL");
                MessageXML = _DBC.LookupWithKey(CONF_API + "_CONF_XML_URL");

                //Get the user list to fetch their mobile numbers
                List<int> nUList = new List<int>();
                nUList.Add(userId);

                foreach (User cUser in userList)
                {
                    nUList.Add(cUser.UserId);
                }

                var tmpUserList = (from U in _context.Set<User>()
                                   where U.CompanyId == companyId && nUList.Contains(U.UserId) && U.Status == 1
                                   select new { UserId = U.UserId, ISD = U.Isdcode, PhoneNumber = U.MobileNo, U.Llisdcode, U.Landline }).Distinct().ToList();


                //Create conference header
                int ConfHeaderId = _MSG.CreateConferenceHeader(companyId, userId, timeZoneId, CommsAPI.ConfRoom, RecordConf, activeIncidentId, messageId, objectType);

                string CallId = string.Empty;
                int ModeratorCDId = 0;
                string ModeratorNumber = string.Empty;
                string ModeratorLandline = string.Empty;
                string Status = string.Empty;

                //Loop through with each user and their phone number to make the call
                foreach (var uItem in tmpUserList)
                {
                    string Mobile = _DBC.FormatMobile(uItem.ISD, uItem.PhoneNumber);
                    string Landline = _DBC.FormatMobile(uItem.Llisdcode, uItem.Landline);

                    if (!string.IsNullOrEmpty(uItem.PhoneNumber))
                    {
                        int CallDetaildId = await _MSG.CreateConferenceDetail("ADD", ConfHeaderId, uItem.UserId, Mobile, Landline, CallId.ToString(), "ADDED", timeZoneId, 0);
                        if (uItem.UserId == userId)
                        {
                            ModeratorCDId = CallDetaildId;
                            ModeratorNumber = Mobile;

                            if (!string.IsNullOrEmpty(Landline))
                                ModeratorLandline = Landline;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(ModeratorNumber))
                {
                    string CallStatus = string.Empty;
                    string CalledOn = "MOBILE";

                    CallStatus = MakeConferenceCall(FromNumber, CallBackUrl, MessageXML, CommsAPI, out CallId, ModeratorNumber, ModeratorLandline, out Status, CalledOn);
                    await _MSG.CreateConferenceDetail("UPDATE", 0, 0, "", "", CallId.ToString(), CallStatus, "", ModeratorCDId, CalledOn);
                }
                return Status;
            }
            catch (Exception ex)
            {
            }
            return "";
        }

        public string MakeConferenceCall(string fromNumber, string callBackUrl, string messageXML, dynamic commsAPI, out string callId,
            string mobileNumber, string landLineNumber, out string status, string calledOn)
        {
            calledOn = "MOBILE";
            string CallStatus = string.Empty;
            //Initiate the call to the modrator
            Task<dynamic> calltask = Task.Factory.StartNew(() => commsAPI.Call(fromNumber, mobileNumber, messageXML, callBackUrl));
            CommsStatus callrslt = calltask.Result;
            callId = callrslt.CommsId;

            if (!callrslt.Status)
            {
                status = "";
                CallStatus = callId = _DBC.Left(callId, 50);
                if (!string.IsNullOrEmpty(landLineNumber))
                {
                    Task<dynamic> recalltask = Task.Factory.StartNew(() => commsAPI.Call(fromNumber, landLineNumber, messageXML, callBackUrl));
                    CommsStatus recallrslt = recalltask.Result;
                    if (recallrslt.Status)
                    {
                        calledOn = "LANDLINE";
                        CallStatus = recallrslt.CurrentAction;
                        status = recallrslt.CurrentAction;
                    }
                    else
                    {
                        callId = recallrslt.CommsId;
                        status = "";
                        CallStatus = callId = _DBC.Left(callId, 50);
                    }
                }
            }
            else
            {
                status = callrslt.CurrentAction;
                CallStatus = callrslt.CurrentAction;
            }
            return CallStatus;
        }

        public string SendOTP(string iSD, string mobileNo, string message, string source = "RESET", string method = "TEXT", string userEmail = "")
        {
            try
            {
                Random rand = new Random();
                int UnqNumber = 1234;

                if (source == "LOGIN")
                {
                    UnqNumber = rand.Next(100000, 999999);
                }
                else
                {
                    UnqNumber = rand.Next(1000, 9999);
                }

                message = message.Replace("{OTP}", UnqNumber.ToString());

                //string Callback = "";

                //if(Source == "LOGIN")
                //    Callback = DBC.LookupWithKey("TWO_FACTOR_SMS_CALLBACK");

                string status = "";
                CommsStatus msgstatus = new CommsStatus();

                if (method == "TEXT")
                { //Send Verificaiton Code Text
                    msgstatus = TwilioVerify(mobileNo, "sms");
                    status = msgstatus.CurrentAction;
                }
                else if (method == "PHONE")
                { // Send verification code by Phone

                    msgstatus = TwilioVerify(mobileNo, "call");
                    status = msgstatus.CurrentAction;

                }
                else if (method == "EMAIL")
                { //Send Verification code by Email
                    string FromEmail = _DBC.LookupWithKey("EMAILFROM");
                    string SMTPHost = _DBC.LookupWithKey("SMTPHOST");

                    string[] Email = { userEmail };
                    bool sent = _SDE.Email(Email, message, FromEmail, SMTPHost, "Crises Control Verification Code");
                    msgstatus.CommsId = "EMAIL";
                    status = "SENT";
                }

                if (status.ToUpper() == "QUEUED" || status.ToUpper() == "SENT" || status.ToUpper() == "ACCEPTED" || status == "PENDING")
                {
                    if (source.ToUpper() == "LOGIN")
                    {
                        LogTwoFactorAuth(GCompanyId, GUserId, mobileNo, msgstatus.CommsId, status, GTimezoneId);
                    }
                    if (status == "PENDING")
                        return "TWLVERIFY";

                    return UnqNumber.ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public async void LogTwoFactorAuth(int companyId, int userId, string toNumber, string cloudMessageId, string status, string timeZoneId)
        {
            try
            {

                TwoFactorAuthLog TFA = new TwoFactorAuthLog();
                TFA.CompanyId = companyId;
                TFA.UserId = userId;
                TFA.ToNumber = toNumber;
                TFA.CloudMessageId = cloudMessageId;
                TFA.LogCollected = false;
                TFA.IsBilled = false;
                TFA.Status = status;
                TFA.CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                _context.Set<TwoFactorAuthLog>().Add(TFA);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
        }

        public CommsStatus SendText(string iSD, string toNumber, string message, string callbackUrl = "")
        {
            CommsStatus textrslt = new CommsStatus();
            try
            {
                string TwilioRoutingApi = string.Empty;
                string FromNumber = _DBC.LookupWithKey("TWILIO_FROM_NUMBER");

                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                var Confs = (from L in _context.Set<SysParameter>() where L.Name == "USE_MESSAGING_COPILOT" || L.Name == "MESSAGING_COPILOT_SID" select L).ToList();

                bool USE_MESSAGING_COPILOT = Convert.ToBoolean(Confs.Where(w => w.Name == "USE_MESSAGING_COPILOT").Select(s => s.Value).FirstOrDefault());
                string MESSAGING_COPILOT_SID = Confs.Where(w => w.Name == "MESSAGING_COPILOT_SID").Select(s => s.Value).FirstOrDefault();

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.USE_MESSAGING_COPILOT = USE_MESSAGING_COPILOT;
                CommsAPI.MESSAGING_COPILOT_SID = MESSAGING_COPILOT_SID;
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                string ClMessageId = string.Empty;
                string istextsend = "NOTSENT";

                //Getting the from number based on the destination.
                var FromNum = (from F in _context.Set<PhoneNumberMapping>() where F.CountryDialCode == iSD select F).FirstOrDefault();
                if (FromNum != null)
                {
                    FromNumber = FromNum.FromNumber;
                }

                toNumber = _DBC.FixMobileZero(toNumber);

                textrslt = CommsAPI.Text(FromNumber, toNumber, message, callbackUrl);
                if (textrslt != null)
                {
                    istextsend = textrslt.CurrentAction;
                    ClMessageId = textrslt.CommsId;
                }
                return textrslt;
            }
            catch (Exception ex)
            {
                textrslt.CurrentAction = "ERROR";
                return textrslt;
            }
        }

        public CommsStatus TwilioVerify(string toNumber, string method)
        {
            CommsStatus textrslt = new CommsStatus();
            try
            {
                string TwilioRoutingApi = string.Empty;

                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                string TWILIO_VERIFY_SERVICEID = _DBC.LookupWithKey("TWILIO_VERIFY_SERVICEID");

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.VERIFY_SERVICE_ID = TWILIO_VERIFY_SERVICEID;
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                string ClMessageId = string.Empty;
                string istextsend = "NOTSENT";

                toNumber = _DBC.FormatMobile("", _DBC.FixMobileZero(toNumber));

                textrslt = CommsAPI.Verify(toNumber, method);

                if (textrslt != null)
                {
                    istextsend = textrslt.CurrentAction;
                    ClMessageId = textrslt.CommsId;
                }
                return textrslt;
            }
            catch (Exception ex)
            {
                textrslt.CurrentAction = "ERROR";
                return textrslt;
            }
        }

        public CommsStatus TwilioVerifyCheck(string toNumber, string code)
        {
            CommsStatus textrslt = new CommsStatus();
            try
            {
                string TwilioRoutingApi = string.Empty;

                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);

                TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                string TWILIO_VERIFY_SERVICEID = _DBC.LookupWithKey("TWILIO_VERIFY_SERVICEID");

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.VERIFY_SERVICE_ID = TWILIO_VERIFY_SERVICEID;
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                string ClMessageId = string.Empty;
                string istextsend = "NOTSENT";

                toNumber = _DBC.FormatMobile("", _DBC.FixMobileZero(toNumber));

                textrslt = CommsAPI.VerifyCheck(toNumber, code);

                if (textrslt != null)
                {
                    istextsend = textrslt.CurrentAction;
                    ClMessageId = textrslt.CommsId;
                }
                return textrslt;
            }
            catch (Exception ex)
            {
                textrslt.CurrentAction = "ERROR";
                return textrslt;
            }
        }

        public void DeleteRecording(string recordingID)
        {
            try
            {
                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                string TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                CommsAPI.DeleteRecording(recordingID);
            }
            catch (Exception)
            {

            }
        }

        public string VerifyPhone(string code, string iSD, string mobileNo, string message = "")
        {
            try
            {

                string validation_method = _DBC.LookupWithKey("PHONE_VALIDATION_METHOD");
                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                string TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                if (string.IsNullOrEmpty(message))
                    message = _DBC.LookupWithKey("PHONE_VALIDATION_MSG");

                message = message.Replace("{OTP}", code).Replace("{CODE}", code);

                mobileNo = _DBC.FixMobileZero(mobileNo);

                if (validation_method == "TEXT")
                {
                    CommsStatus status = SendText(iSD, mobileNo, message);
                    return status.CurrentAction;
                }
                else
                {
                    CommsStatus status = VerificationCall(mobileNo, message, SendInDirect, TwilioRoutingApi);
                    return status.CommsId;
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        internal CommsStatus VerificationCall(string mobileNo, string message, bool sendInDirect, string twilioRoutingApi)
        {
            try
            {

                string FromNumber = _DBC.LookupWithKey("TWILIO_FROM_NUMBER");
                string MsgXML = _DBC.LookupWithKey("TWILIO_PHONE_VALIDATION_XML");
                MsgXML = MsgXML + "?Body=" + HttpUtility.UrlEncode(message);

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.IsConf = false;
                CommsAPI.SendInDirect = sendInDirect;
                CommsAPI.TwilioRoutingApi = twilioRoutingApi;

                Task<dynamic> trhtask = Task.Factory.StartNew(() => CommsAPI.Call(FromNumber, mobileNo, MsgXML, MsgXML));
                CommsStatus callrslt = trhtask.Result;
                return callrslt;
            }
            catch (Exception ex)
            {
                return new CommsStatus();
            }
        }

        public string CallMe(string ISD, string mobileNo)
        {
            try
            {

                string FromNumber = _DBC.LookupWithKey("TWILIO_FROM_NUMBER");
                string MsgXML = _DBC.LookupWithKey("TWILIO_CALLME_XML");
                string SALES_SUPPORT_NO = _DBC.LookupWithKey("SALES_NUMBER");
                bool SendInDirect = _DBC.IsTrue(_DBC.LookupWithKey("TWILIO_USE_INDIRECT_CONNECTION"), false);
                string TwilioRoutingApi = _DBC.LookupWithKey("TWILIO_ROUTING_API");

                MsgXML = MsgXML + "?ConfRoom=CALLME_" + DateTime.Now.ToString("ddMMyyHHmm");
                string CustomerURL = MsgXML + "&IsCustomer=true";

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.IsConf = false;
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                mobileNo = _DBC.FixMobileZero(mobileNo);

                Task<dynamic> trhtask = Task.Factory.StartNew(() => CommsAPI.Call(FromNumber, mobileNo, CustomerURL, CustomerURL));
                CommsStatus callrsltcustomer = trhtask.Result;

                Task<dynamic> trhtasksales = Task.Factory.StartNew(() => CommsAPI.Call(FromNumber, SALES_SUPPORT_NO, MsgXML, MsgXML));
                CommsStatus callrsltsales = trhtasksales.Result;
                return callrsltsales.CommsId;

            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public dynamic InitComms(string API_CLASS, string APIClass = "", string clientId = "", string clientSecret = "", string dataCenter = "dublin")
        {
            try
            {

                int RetryCount = 2;
                int.TryParse(_DBC.LookupWithKey(API_CLASS + "_MESSAGE_RETRY_COUNT"), out RetryCount);

                if (string.IsNullOrEmpty(APIClass))
                    APIClass = _DBC.LookupWithKey(API_CLASS + "_API_CLASS");

                if (string.IsNullOrEmpty(clientId))
                    clientId = _DBC.LookupWithKey(API_CLASS + "_CLIENTID");

                if (string.IsNullOrEmpty(clientSecret))
                    clientSecret = _DBC.LookupWithKey(API_CLASS + "_CLIENT_SECRET");

                string[] TmpClass = APIClass.Trim().Split('|');

                string binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");

                Assembly assembly = Assembly.LoadFrom(binPath + "\\" + TmpClass[0]);
                Type type = assembly.GetType(TmpClass[1]);
                dynamic CommsAPI = Activator.CreateInstance(type);

                CommsAPI.ClientId = clientId;
                CommsAPI.Secret = clientSecret;
                CommsAPI.RetryCount = RetryCount;

                return CommsAPI;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }

}
