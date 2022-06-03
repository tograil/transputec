using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register;
using CrisesControl.Core.Register.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<RegisterRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RegisterRepository(ILogger<RegisterRepository> logger, CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
          this._logger = logger;
          this._context = context;
            this._httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> CheckCustomer(string CustomerId)
        {
            try
            {
                CustomerId = CustomerId.Trim().ToLower();
                var Customer = await _context.Set<Company>()
                                     .Include(c=>c.StdTimeZone)
                                     .Where(C => C.CustomerId == CustomerId)
                                     .AnyAsync();
                return Customer;
            }
            catch (Exception ex)
            {
               _logger.LogError("Error occured while seeding into database {0}, {1}",ex.Message,ex.InnerException);
                return false;
            }
        }

        public async Task<CommsStatus> SendText(string ISD, string ToNumber, string Message, string CallbackUrl = "")
        {
            try
            {
                CommsStatus textrslt = new CommsStatus();
                string TwilioRoutingApi = string.Empty;
                string FromNumber = LookupWithKey(KeyType.TWILIOFROMNUMBER.ToDbKeyString());

                bool SendInDirect = IsTrue(LookupWithKey(KeyType.TWILIOUSEINDIRECTCONNECTION.ToDbKeyString()), false);

                TwilioRoutingApi = LookupWithKey(KeyType.TWILIOROUTINGAPI.ToDbKeyString());

                var Confs = await  _context.Set<SysParameter>().Where(L=> L.Name ==KeyType.USEMESSAGINGCOPILOT.ToDbKeyString() || L.Name == KeyType.MESSAGINGCOPILOTSID.ToDbKeyString()).ToListAsync();

                bool USE_MESSAGING_COPILOT = Convert.ToBoolean(Confs.Where(w => w.Name == "USE_MESSAGING_COPILOT").Select(s => s.Value).FirstOrDefault());
                string MESSAGING_COPILOT_SID =Confs.Where(w => w.Name == "MESSAGING_COPILOT_SID").Select(s => s.Value).FirstOrDefault().ToString();

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.USE_MESSAGING_COPILOT = USE_MESSAGING_COPILOT;
                CommsAPI.MESSAGING_COPILOT_SID = MESSAGING_COPILOT_SID;
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                string ClMessageId = string.Empty;
                string istextsend = "NOTSENT";

                //Getting the from number based on the destination.
                var FromNum = await _context.Set<PhoneNumberMapping>().Where(F=> F.CountryDialCode == ISD).FirstOrDefaultAsync();
                if (FromNum != null)
                {
                    FromNumber =  FromNum.FromNumber;
                }

                ToNumber = ToNumber.FixMobileZero();

                textrslt = CommsAPI.Text(FromNumber, ToNumber, Message, CallbackUrl);
                if (textrslt != null)
                {
                    istextsend = textrslt.CurrentAction;
                    ClMessageId = textrslt.CommsId;
                }
                return textrslt;
            }
            catch (Exception ex)
            {

                return new CommsStatus
                {
                    CurrentAction = "Error"
                };
            }
        }

        public async Task<string> VerifyPhone(string Code, string ISD, string MobileNo, string Message = "")
        {
            try
            {

                string validation_method = LookupWithKey(KeyType.PHONEVALIDATIONMETHOD.ToDbKeyString());
                bool SendInDirect = IsTrue(LookupWithKey(KeyType.TWILIOUSEINDIRECTCONNECTION.ToDbKeyString()), false);
                string TwilioRoutingApi = LookupWithKey(KeyType.TWILIOROUTINGAPI.ToDbKeyString());

                if (string.IsNullOrEmpty(Message))
                    Message = LookupWithKey(KeyType.PHONEVALIDATIONMSG.ToDbKeyString());

                Message = Message.Replace("{OTP}", Code).Replace("{CODE}", Code);

                MobileNo = MobileNo.FixMobileZero();

                if (validation_method == MessageType.Text.ToDbString())
                {
                    CommsStatus status = await SendText(ISD, MobileNo, Message);
                    return status.CurrentAction;
                }
                else
                {
                    CommsStatus status =await VerificationCall(MobileNo, Message, SendInDirect, TwilioRoutingApi);
                    return status.CommsId;
                }
            }
            catch (Exception ex)
            {
               throw ex;
                return "";
            }
        }

        public async Task<CommsStatus> VerificationCall(string mobileNo, string message, bool sendInDirect, string twilioRoutingApi)
        {
            try
            {

                string FromNumber = LookupWithKey("TWILIO_FROM_NUMBER");
                string MsgXML = LookupWithKey("TWILIO_PHONE_VALIDATION_XML");
                // TODO: still going to investigate this
               // MsgXML = MsgXML + "?Body=" + _httpContextAccessor.HttpContext.Server.UrlEncode(message);

                dynamic CommsAPI = this.InitComms("TWILIO");
                CommsAPI.IsConf = false;
                CommsAPI.SendInDirect = sendInDirect;
                CommsAPI.TwilioRoutingApi = twilioRoutingApi;

                CommsStatus trhtask = Task.Factory.StartNew(() => CommsAPI.Call(FromNumber, mobileNo, MsgXML, MsgXML)).Result;
                
                return trhtask;
            }
            catch (Exception ex)
            {
                 return  new CommsStatus();
            }
        }

        private bool IsTrue(string BoolVal, bool Default = true)
        {
            if (BoolVal == true.ToString())
            {
                return true;
            }
            else if (BoolVal == false.ToString())
            {
                return false;
            }
            else
            {
                return Default;
            }
        }
        private string LookupWithKey(string Key, string Default = "")
        {
            try
            {
                var LKP = (from L in _context.Set<SysParameter>()
                           where L.Name == Key
                           select L).FirstOrDefault();
                if (LKP != null)
                {
                    Default = LKP.Value;
                }
                return Default;
            }
            catch (Exception ex)
            {
                return Default;
            }
        }
        public dynamic InitComms(string API_CLASS, string APIClass = "", string ClientId = "", string ClientSecret = "")
        {
            
            try
            {

                int RetryCount = 2;
                int.TryParse(LookupWithKey(API_CLASS + KeyType.MESSAGERETRYCOUNT.ToDbKeyString()), out RetryCount);

                if (string.IsNullOrEmpty(APIClass))
                    APIClass = LookupWithKey(API_CLASS + KeyType.APICLASS.ToDbKeyString());

                if (string.IsNullOrEmpty(ClientId))
                    ClientId = LookupWithKey(API_CLASS + KeyType._CLIENTID.ToDbKeyString());

                if (string.IsNullOrEmpty(ClientSecret))
                    ClientSecret = LookupWithKey(API_CLASS + KeyType.CLIENTSECRET.ToDbKeyString());

                string[] TmpClass = APIClass.Trim().Split('|');

                string binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");

                Assembly assembly = Assembly.LoadFrom(binPath + "\\" + TmpClass[0]);
                Type type = assembly.GetType(TmpClass[1]);
                dynamic CommsAPI = Activator.CreateInstance(type);

                CommsAPI.ClientId = ClientId;
                CommsAPI.Secret = ClientSecret;
                CommsAPI.RetryCount = RetryCount;

                return CommsAPI;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }
    }
}
