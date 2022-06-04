using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Messages.Services;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register;
using CrisesControl.Core.Register.Repositories;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IMessageService _messageService;
        private readonly ICompanyRepository _companyRepository;
        private int UserID;
        private int CompanyId;
        public RegisterRepository(ILogger<RegisterRepository> logger, IMessageService service, CrisesControlContext context, IHttpContextAccessor httpContextAccessor)
        {
          this._logger = logger;
          this._context = context;
          this._httpContextAccessor = httpContextAccessor;
          this._messageService  = service;

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

        public async Task<string> ValidateMobile(string Code, string ISD, string MobileNo, string Message = "")
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

        public async Task<User> ValidateUserEmail(string uniqueId, int companyId)
        {
            try
            {
              

                var UserInfo =await _context.Set<User>().Where(u=>u.CompanyId == companyId && u.UniqueGuiId == uniqueId
                                ).FirstOrDefaultAsync();

                if (UserInfo != null)
                {
                    _context.Update(UserInfo);
                   await  _context.SaveChangesAsync();
                    return UserInfo;

                    
                }
            
            
            }
           
            catch (Exception ex)
            {
                _logger.LogError("Error occured whi seeding into the database");
                
            }
            return new User { };
        }
        public async Task CreateObjectRelationship(int TargetObjectID, int SourceObjectID, string RelationName, int CompanyId, int CreatedUpdatedBy, string TimeZoneId, string RelatinFilter = "")
        {
            try
            {
              

                if (RelationName.ToUpper() == "GROUP" || RelationName.ToUpper() == "LOCATION")
                {
                    if (TargetObjectID > 0)
                    {
                        int NewSourceObjectID = 0;

                        var ObjMapId = await _context.Set<ObjectMapping>().Include(om => om.Object).Where(OBJ => OBJ.Object.ObjectName == RelationName).Select(a => a.ObjectMappingId).FirstOrDefaultAsync();

                        if (SourceObjectID > 0)
                        {
                            NewSourceObjectID = SourceObjectID;
                            await CreateNewObjectRelation(NewSourceObjectID, TargetObjectID, ObjMapId, CreatedUpdatedBy, TimeZoneId);
                        }

                        if (!string.IsNullOrEmpty(RelatinFilter))
                        {
                            if (RelationName.ToUpper() == "GROUP")
                            {
                                NewSourceObjectID = await _context.Set<Group>().Where(D=> D.GroupName == RelatinFilter && D.CompanyId == CompanyId).Select(g=>g.GroupId).FirstOrDefaultAsync();
                            }
                            else if (RelationName.ToUpper() == "LOCATION")
                            {
                                NewSourceObjectID = await  _context.Set<Location>().Where(L=> L.LocationName == RelatinFilter && L.CompanyId == CompanyId).Select(L=>L.LocationId).FirstOrDefaultAsync();
                            }
                            await CreateNewObjectRelation(NewSourceObjectID, TargetObjectID, ObjMapId, CreatedUpdatedBy, TimeZoneId);
                        }
                    }
                }
                else if (RelationName.ToUpper() == "DEPARTMENT")
                {
                  await  UpdateUserDepartment(SourceObjectID, CreatedUpdatedBy, TimeZoneId);
                }
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public async Task CreateNewObjectRelation(int SourceObjectID, int TargetObjectID, int ObjMapId, int CreatedUpdatedBy, string TimeZoneId)
        {
            try
            {

                bool IsALLOBJrelationExist = await _context.Set<ObjectRelation>().Where(OBR=> OBR.TargetObjectPrimaryId == TargetObjectID
                                              && OBR.ObjectMappingId == ObjMapId
                                              && OBR.SourceObjectPrimaryId == SourceObjectID).AnyAsync();
                if (!IsALLOBJrelationExist)
                {
                    ObjectRelation tblDepObjRel = new ObjectRelation()
                    {
                        TargetObjectPrimaryId = TargetObjectID,
                        ObjectMappingId = ObjMapId,
                        SourceObjectPrimaryId = SourceObjectID,
                        CreatedBy = CreatedUpdatedBy,
                        UpdatedBy = CreatedUpdatedBy,
                        CreatedOn = System.DateTime.Now,
                        UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(TimeZoneId, System.DateTime.Now),
                        ReceiveOnly = false
                    };
                    await _context.AddAsync(tblDepObjRel);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task UpdateUserDepartment(int DepartmentID, int CreatedUpdatedBy, string TimeZoneId)
        {
            try
            {
                UserID =Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst("sub"));
                var user = await _context.Set<User>().Where(U=> U.UserId == UserID).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.DepartmentId = DepartmentID;
                    user.UpdatedBy = CreatedUpdatedBy;
                    user.UpdatedOn = DateTime.Now.GetDateTimeOffset(TimeZoneId);
                   await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task NewUserAccountConfirm(string EmailId, string UserName, string UserPass, int CompanyId, string guid)
        {
            try
            {

                //string path = Convert.ToString(DBC.LookupWithKey("API_TEMPLATE_PATH")) + "NewAccountConfirmed.html";

                
                string Subject = string.Empty;
                string message = Convert.ToString(await ReadHtmlFile("NEW_ACCOUNT_CONFIRMED", "DB", CompanyId,  Subject));

                var CompanyInfo =await  _context.Set<Company>().Where(C=> C.CompanyId == CompanyId ).FirstOrDefaultAsync();
                string Website = LookupWithKey("DOMAIN");
                string Portal = LookupWithKey("PORTAL");
                string ValdiateURL = LookupWithKey("EMAIL_VALIDATE_URL");
                string hostname = LookupWithKey("SMTPHOST");
                string fromadd = LookupWithKey("EMAILFROM");
                string sso_login = await _companyRepository.GetCompanyParameter("AAD_SSO_TENANT_ID", CompanyId);

                string Verifylink = Portal + ValdiateURL + CompanyId + "/" + guid;
                string CompanyLogo = Portal + "/uploads/" + CompanyInfo.CompanyId + "/companylogos/" + CompanyInfo.CompanyLogoPath;

                if (string.IsNullOrEmpty(CompanyInfo.CompanyLogoPath))
                {
                    CompanyLogo = LookupWithKey("CCLOGO");
                }

                if (!string.IsNullOrEmpty(sso_login))
                    UserPass = "Use the single sign-on to login";

                if ((message != null) && (hostname != null) && (fromadd != null))
                {
                    string messagebody = message;

                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", UserName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", EmailId);
                    messagebody = messagebody.Replace("{RECIPIENT_PASSWORD}", UserPass);
                    messagebody = messagebody.Replace("{COMPANY_NAME}", CompanyInfo.CompanyName);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                    messagebody = messagebody.Replace("{VERIFY_LINK}", Verifylink);
                    messagebody = messagebody.Replace("{CC_WEBSITE}", Website);
                    messagebody = messagebody.Replace("{PORTAL}", Portal);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", CompanyInfo.CustomerId);

                    messagebody = messagebody.Replace("{TWITTER_LINK}", LookupWithKey("CC_TWITTER_PAGE"));
                    messagebody = messagebody.Replace("{TWITTER_ICON}", LookupWithKey("CC_TWITTER_ICON"));
                    messagebody = messagebody.Replace("{FACEBOOK_LINK}", LookupWithKey("CC_FB_PAGE"));
                    messagebody = messagebody.Replace("{FACEBOOK_ICON}",LookupWithKey("CC_FB_ICON"));
                    messagebody = messagebody.Replace("{LINKEDIN_LINK}", LookupWithKey("CC_LINKEDIN_PAGE"));
                    messagebody = messagebody.Replace("{LINKEDIN_ICON}", LookupWithKey("CC_LINKEDIN_ICON"));

                    string[] toEmails = { EmailId };

                    bool ismailsend = Email(toEmails, messagebody, fromadd, hostname, Subject);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<StringBuilder> ReadHtmlFile(string FileCode, string Source, int CompanyId, string Subject, string Provider = "AWSSES")
        {
            StringBuilder htmlContent = new StringBuilder();
            string line;
            Subject = "";
            try
            {
                if (Source == "FILE")
                {
                    using (StreamReader htmlReader = new System.IO.StreamReader(FileCode))
                    {
                        while ((line = htmlReader.ReadLine()) != null)
                        {
                            htmlContent.Append(line);
                        }
                    }
                }
                else
                {


                    var content = await _context.Set<EmailTemplate>()
                                .Where(MSG => MSG.CompanyId == 0 && MSG.Code == FileCode)
                                .Union(_context.Set<EmailTemplate>()
                                .Where(MSG => MSG.Code == FileCode && MSG.CompanyId == CompanyId))
                                .OrderByDescending(MSG => MSG.CompanyId).FirstOrDefaultAsync();
                    {
                        Subject = content.EmailSubject;

                        var head = await _context.Set<EmailTemplate>().Where(MSG => MSG.Code == "DOCHEAD").FirstOrDefaultAsync();
                        if (head != null)
                        {
                            htmlContent.AppendLine(head.HtmlData.ToString());
                        }

                        htmlContent.Append(content.HtmlData.ToString());

                        if (Provider.ToUpper() != "OFFICE365")
                        {
                            var desc = await _context.Set<EmailTemplate>()
                                .Where(MSG => MSG.CompanyId == 0 && MSG.Code == "DISCLAIMER_TEXT")
                                .Union(_context.Set<EmailTemplate>()
                                .Where(MSG => MSG.Code == "DISCLAIMER_TEXT" && MSG.CompanyId == CompanyId))
                                .OrderByDescending(MSG => MSG.CompanyId).FirstOrDefaultAsync();

                            if (desc != null)
                            {
                                htmlContent.Append(desc.HtmlData.ToString());
                            }
                        }
                        htmlContent.AppendLine("</body></html>");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return htmlContent;
        }

    }
}
