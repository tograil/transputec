using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Exceptions.NotFound.Base;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register;
using CrisesControl.Core.Register.Repositories;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CrisesControl.Infrastructure.Repositories
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<RegisterRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIncidentRepository _incidentRepository;

        private readonly ICompanyRepository _companyRepository;
        private readonly ISenderEmailService _senderEmail;
        private readonly DBCommon _DBC;
    
        private int UserID;
        private int CompanyId;
        public RegisterRepository(ILogger<RegisterRepository> logger, ISenderEmailService senderEmail,  
            CrisesControlContext context, IHttpContextAccessor httpContextAccessor, IIncidentRepository incidentRepository,
            ICompanyRepository companyRepository, DBCommon DBC)
        {
          this._logger = logger;
          this._context = context;
          this._httpContextAccessor = httpContextAccessor;
          this._senderEmail=senderEmail;
          this._incidentRepository=incidentRepository;
            this._companyRepository = companyRepository;
            _DBC = DBC;
      

        }
        public async Task<List<Registration>> GetAllRegistrations()
        {
            return await _context.Set<Registration>().ToListAsync();
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
                string FromNumber = await LookupWithKey(KeyType.TWILIOFROMNUMBER.ToDbKeyString());

                bool SendInDirect = IsTrue(await LookupWithKey(KeyType.TWILIOUSEINDIRECTCONNECTION.ToDbKeyString()), false);

                TwilioRoutingApi = await LookupWithKey(KeyType.TWILIOROUTINGAPI.ToDbKeyString());

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

                string validation_method = await LookupWithKey(KeyType.PHONEVALIDATIONMETHOD.ToDbKeyString());
                bool SendInDirect = IsTrue(await LookupWithKey(KeyType.TWILIOUSEINDIRECTCONNECTION.ToDbKeyString()), false);
                string TwilioRoutingApi = await LookupWithKey(KeyType.TWILIOROUTINGAPI.ToDbKeyString());

                if (string.IsNullOrEmpty(Message))
                    Message = await LookupWithKey(KeyType.PHONEVALIDATIONMSG.ToDbKeyString());

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

                string FromNumber =await LookupWithKey("TWILIO_FROM_NUMBER");
                string MsgXML = await LookupWithKey("TWILIO_PHONE_VALIDATION_XML");
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
        private async Task<string> LookupWithKey(string Key, string Default = "")
        {
            try
            {
                var LKP = await _context.Set<SysParameter>()
                           .Where(L=> L.Name == Key
                           ).FirstOrDefaultAsync();
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
        public async Task<dynamic> InitComms(string API_CLASS, string APIClass = "", string ClientId = "", string ClientSecret = "")
        {
            
            try
            {

                int RetryCount = 2;
                int.TryParse(await LookupWithKey(API_CLASS + KeyType.MESSAGERETRYCOUNT.ToDbKeyString()), out RetryCount);

                if (string.IsNullOrEmpty(APIClass))
                    APIClass =await LookupWithKey(API_CLASS + KeyType.APICLASS.ToDbKeyString());

                if (string.IsNullOrEmpty(ClientId))
                    ClientId = await LookupWithKey(API_CLASS + KeyType._CLIENTID.ToDbKeyString());

                if (string.IsNullOrEmpty(ClientSecret))
                    ClientSecret = await LookupWithKey(API_CLASS + KeyType.CLIENTSECRET.ToDbKeyString());

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
                string message = Convert.ToString( ReadHtmlFile("NEW_ACCOUNT_CONFIRMED", "DB", CompanyId,  out Subject));

                var CompanyInfo =await  _context.Set<Company>().Where(C=> C.CompanyId == CompanyId ).FirstOrDefaultAsync();
                string Website = await LookupWithKey("DOMAIN");
                string Portal = await LookupWithKey("PORTAL");
                string ValdiateURL = await LookupWithKey("EMAIL_VALIDATE_URL");
                string hostname =await LookupWithKey("SMTPHOST");
                string fromadd = await LookupWithKey("EMAILFROM");
                string sso_login = await _companyRepository.GetCompanyParameter("AAD_SSO_TENANT_ID", CompanyId);

                string Verifylink = Portal + ValdiateURL + CompanyId + "/" + guid;
                string CompanyLogo = Portal + "/uploads/" + CompanyInfo.CompanyId + "/companylogos/" + CompanyInfo.CompanyLogoPath;

                if (string.IsNullOrEmpty(CompanyInfo.CompanyLogoPath))
                {
                    CompanyLogo = await LookupWithKey("CCLOGO");
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

                    messagebody = messagebody.Replace("{TWITTER_LINK}", await LookupWithKey("CC_TWITTER_PAGE"));
                    messagebody = messagebody.Replace("{TWITTER_ICON}", await LookupWithKey("CC_TWITTER_ICON"));
                    messagebody = messagebody.Replace("{FACEBOOK_LINK}", await LookupWithKey("CC_FB_PAGE"));
                    messagebody = messagebody.Replace("{FACEBOOK_ICON}", await LookupWithKey("CC_FB_ICON"));
                    messagebody = messagebody.Replace("{LINKEDIN_LINK}", await LookupWithKey("CC_LINKEDIN_PAGE"));
                    messagebody = messagebody.Replace("{LINKEDIN_ICON}", await LookupWithKey("CC_LINKEDIN_ICON"));

                    string[] toEmails = { EmailId };

                    bool ismailsend =await _senderEmail.Email(toEmails, messagebody, fromadd, hostname, Subject);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  StringBuilder ReadHtmlFile(string FileCode, string Source, int CompanyId, out string Subject, string Provider = "AWSSES")
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


                    var content =  _context.Set<EmailTemplate>()
                                .Where(MSG => MSG.CompanyId == 0 && MSG.Code == FileCode)
                                .Union(_context.Set<EmailTemplate>()
                                .Where(MSG => MSG.Code == FileCode && MSG.CompanyId == CompanyId))
                                .OrderByDescending(MSG => MSG.CompanyId).FirstOrDefault();
                    {
                        Subject = content.EmailSubject;

                        var head =  _context.Set<EmailTemplate>().Where(MSG => MSG.Code == "DOCHEAD").FirstOrDefault();
                        if (head != null)
                        {
                            htmlContent.AppendLine(head.HtmlData.ToString());
                        }

                        htmlContent.Append(content.HtmlData.ToString());

                        if (Provider.ToUpper() != "OFFICE365")
                        {
                            var desc =  _context.Set<EmailTemplate>()
                                .Where(MSG => MSG.CompanyId == 0 && MSG.Code == "DISCLAIMER_TEXT")
                                .Union(_context.Set<EmailTemplate>()
                                .Where(MSG => MSG.Code == "DISCLAIMER_TEXT" && MSG.CompanyId == CompanyId))
                                .OrderByDescending(MSG => MSG.CompanyId).FirstOrDefault();

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
        public async Task<bool> UpgradeRequest(int CompanyId)
        {
            try
            {
                string Subject = string.Empty;
                string template = "PLAN_UPGRADE_REQ";

                //string path = Convert.ToString(DBC.LookupWithKey("API_TEMPLATE_PATH"));

                var sysparms = await _context.Set<SysParameter>()
                                .Where(SP=> SP.Name == "DOMAIN" || SP.Name == "PORTAL" || SP.Name == "SMTPHOST"
                                || SP.Name == "EMAILFROM" || SP.Name == "ADMIN_SITE_URL"
                                ).ToListAsync();


                var Company = await _context.Set<Company>().Where(C=> C.CompanyId == CompanyId).FirstOrDefaultAsync();
                string Website = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault();
                string Portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault();
                string AdminPortal = sysparms.Where(w => w.Name == "ADMIN_SITE_URL").Select(s => s.Value).FirstOrDefault();
                string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault();
                string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault();

                string message = Convert.ToString( ReadHtmlFile(template, "DB", CompanyId,  out Subject));

                if ((hostname != null) && (fromadd != null))
                {

                    StringBuilder adminMsg = new StringBuilder();

                    adminMsg.AppendLine("<h2>An upgrade request has been made by customer</h2>");
                    adminMsg.AppendLine("<p>Below are the company information:</p>");
                    adminMsg.AppendLine("<strong>Company ID: </strong>CC" + Company.CompanyId + "</br>");
                    adminMsg.AppendLine("<strong>Company Name: </strong>" + Company.CompanyName + "</br>");
                    adminMsg.AppendLine("<strong>Registered On: </strong>" + Company.RegistrationDate.ToString("dd-MM-yy HH:mm") + "</br>");

                    adminMsg.AppendLine("<p style=\"color:#ff0000;font-size:18px\"><strong><a href=\"" + AdminPortal + "\">Click here to login to admin portal to view company details</a></p>");

                    string[] AdminEmail = (await LookupWithKey("BILLING_EMAIL")).Split(',');

                    await _senderEmail.Email(AdminEmail, adminMsg.ToString(), fromadd, hostname, "Crises Control: " + Company.CompanyName + " requsted for an upgrade");


                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }
        public async Task<int> VerifyTempRegistration(Registration reg)
        {
            try
            {
               
                
                if(reg != null)
                {
                    _context.Update(reg);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("The Registration has been verified" + reg.Id);
                    return reg.Id;
                }
               
               
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while seeding into database {0},{1},{2}", ex.Message, ex.InnerException, ex.StackTrace);
            }
            return 0;

        }
        public async Task<Registration> GetRegistrationByUniqueReference(string UniqueRef)
        {
            var reg = await _context.Set<Registration>().Where(R => R.UniqueReference == UniqueRef ).FirstOrDefaultAsync();
            return reg;
        }
        public async Task<Registration> GetRegistrationDataByEmail(string Email)
        {
            var reg = await _context.Set<Registration>().Where(R =>  R.Email == Email).OrderBy(a => a.Id).FirstOrDefaultAsync();
            return reg;
        }
        public async Task<Registration> TempRegister(Registration reg)
        {
            try
            {

                
                   await _context.AddAsync(reg);
                    await _context.SaveChangesAsync();
               

                return reg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }

        public async Task<bool> SetupCompleted(Company company)
        {
           
            if (company != null)
            {



                _context.Update(company);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<Registration> GetTempRegistration(int RegID, string UniqueRef)
        {
            try
            {
                if (RegID > 0)
                {
                    var reg = await _context.Set<Registration>().Where(R=> R.Id == RegID).FirstOrDefaultAsync();
                    return reg;
                }
                else if (!string.IsNullOrEmpty(UniqueRef))
                {
                    var reg = await _context.Set<Registration>().Where(R=> R.UniqueReference == UniqueRef).FirstOrDefaultAsync();
                    return reg;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while seeding into database {0}, {1},{2}",ex.Message,ex.InnerException,ex.StackTrace);
              
            }
            throw new RegisterNotFoundException(RegID, 0);


        }
        public async Task<bool> DeleteTempRegistration(Registration registration)
        {
            try
            {
                
                if (registration != null)
                {
                    _context.Remove(registration);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new RegisterNotFoundException(0, 0);
            }
            
        }

        public async Task<bool> ActivateCompany(int UserId, string ActivationKey, string IPAddress, int SalesSource = 0, string TimeZoneId = "GMT Standard Time")
        {

            try
            {
                int getuser = await _context.Set<User>().Where(U=> U.UserId == UserId).Select(U=>U.CompanyId).FirstOrDefaultAsync();

                if (getuser > 0)
                {
                    var checkkey =  _context.Set<Company>().Include(CP=>CP.CompanyPaymentProfiles)
                                    .Include(CA=>CA.CompanyActivation)                                   
                                    .Where(CA=> CA.CompanyActivation.ActivationKey == ActivationKey && CA.CompanyId == getuser && CA.Status == 0
                                    ).FirstOrDefault();
                    if (checkkey != null)
                    {
                        checkkey.CompanyActivation.ActivatedBy = UserId;
                        checkkey.CompanyActivation.ActivatedOn = DateTime.Now.GetDateTimeOffset( TimeZoneId);

                        if (checkkey.Status == 4)
                        {
                            checkkey.CompanyProfile = "AWAITING_SETUP";
                        }

                        checkkey.CompanyActivation.Status = 1;
                        checkkey.CompanyActivation.Ipaddress = IPAddress;
                        if (checkkey.CompanyActivation.SalesSource <= 0)
                            checkkey.CompanyActivation.SalesSource = SalesSource;
                        checkkey.Status = 1;
                        _context.Update(checkkey);
                        await _context.SaveChangesAsync();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                throw new CompanyNotFoundException(CompanyId, UserId);
                return false;
            }
        }
        public async Task<UserDevice> GetUserDeviceByUserId(int UserID)
        {
            var device =await  _context.Set<UserDevice>().Where(UD=> UD.UserId == UserID && UD.Status == 1 ).FirstOrDefaultAsync();
            return device;
  
        }
        public async Task<User> GetUserByUniqueId(string UniqueId)
        {
            var data = await _context.Set<User>().Where(U => U.UniqueGuiId == UniqueId)
                      .FirstOrDefaultAsync();
            return data;

        }
        public async Task<CompanyUser> SendVerification(string UniqueId)
        {
            var data = await _context.Set<User>().Include(x=>x.Company).Where(U=>U.UniqueGuiId == UniqueId)
            . Select(U => new CompanyUser()
              {
                  UserId = U.UserId,
                  UserName = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName },
                  UserEmail = U.PrimaryEmail,
                  UniqueID = U.UniqueGuiId,
                  CompanyId = U.Company.CompanyId,
                  TimeZoneId = U.Company.StdTimeZone.ZoneLabel,
              }).FirstOrDefaultAsync();
            return data;

        }
        public async Task NewUserAccount(string EmailId, string UserName, int CompanyId, string guid)
        {
            try
            {
                //string path = Convert.ToString(DBC.LookupWithKey("API_TEMPLATE_PATH")) + "NewUserAccount.html";
                string Subject = string.Empty;

                string message = Convert.ToString(ReadHtmlFile("NEW_USER_ACCOUNT", "DB", CompanyId, out Subject));
                var sysparms = await  _context.Set<SysParameter>().
                                Where(SP=> SP.Name == "CC_TWITTER_PAGE" || SP.Name == "CC_FB_PAGE"
                                || SP.Name == "CC_LINKEDIN_PAGE" || SP.Name == "DOMAIN"
                                || SP.Name == "CC_TWITTER_ICON" || SP.Name == "CC_FB_ICON"
                                || SP.Name == "CC_LINKEDIN_ICON" || SP.Name == "CC_USER_SUPPORT_LINK"
                                || SP.Name == "PORTAL" || SP.Name == "SMTPHOST" || SP.Name == "EMAIL_VALIDATE_URL" || SP.Name == "EMAIL_VALIDATE_ACCOUNT_DELETE"
                                || SP.Name == "EMAILFROM" || SP.Name == "CCLOGO").Select(SP=>new
                                 { SP.Name, SP.Value }).ToListAsync();


                var Company = _context.Set<Company>().Where(C => C.CompanyId == CompanyId).FirstOrDefault();

                string Website = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault();
                string Portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault();
                string ValdiateURL = sysparms.Where(w => w.Name == "EMAIL_VALIDATE_URL").Select(s => s.Value).FirstOrDefault();
                string AccountDeleteURL = sysparms.Where(w => w.Name == "EMAIL_VALIDATE_ACCOUNT_DELETE").Select(s => s.Value).FirstOrDefault();

                string Verifylink = Portal + ValdiateURL + CompanyId + "/" + guid;
                string DeleteVerifyLink = Portal + AccountDeleteURL + CompanyId + "/" + guid;

                string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault();
                string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault();
                string CompanyLogo = Portal + "/uploads/" + CompanyId + "/companylogos/" + Company.CompanyLogoPath;

                if (string.IsNullOrEmpty(Company.CompanyLogoPath))
                {
                    CompanyLogo = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault();
                }


                if ((message != null) && (hostname != null) && (fromadd != null))
                {

                    string messagebody = message;

                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", UserName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", EmailId);
                    messagebody = messagebody.Replace("{COMPANY_NAME}", Company.CompanyName);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                    messagebody = messagebody.Replace("{PORTAL}", Portal);
                    messagebody = messagebody.Replace("{VERIFY_LINK}", Verifylink);
                    messagebody = messagebody.Replace("{DELETE_ACCOUNT_LINK}", DeleteVerifyLink);
                    messagebody = messagebody.Replace("{CC_WEBSITE}", Website);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", Company.CustomerId);

                    messagebody = messagebody.Replace("{TWITTER_LINK}", sysparms.Where(w => w.Name == "CC_TWITTER_PAGE").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{TWITTER_ICON}", sysparms.Where(w => w.Name == "CC_TWITTER_ICON").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{FACEBOOK_LINK}", sysparms.Where(w => w.Name == "CC_FB_PAGE").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{FACEBOOK_ICON}", sysparms.Where(w => w.Name == "CC_FB_ICON").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{LINKEDIN_LINK}", sysparms.Where(w => w.Name == "CC_LINKEDIN_PAGE").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{LINKEDIN_ICON}", sysparms.Where(w => w.Name == "CC_LINKEDIN_ICON").Select(s => s.Value).FirstOrDefault());

                    string[] toEmails = { EmailId };
                    bool ismailsend = await _senderEmail.Email(toEmails, messagebody, fromadd, hostname, Subject);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> UserName(UserFullName strUserName)
        {
            try
            {
                if (strUserName != null)
                {
                    return strUserName.Firstname + " " + strUserName.Lastname;
                }
            }
            catch (Exception ex)
            {
                throw ex;
               
            }
            return "";
        }
        public async Task SendCredentials(string EmailId, string UserName, string UserPass, int CompanyId, string guid)
        {
            try
            {
                string Subject = string.Empty;
                string message = Convert.ToString(ReadHtmlFile("SEND_CREDENTIAL", "DB", CompanyId, out Subject));

                var CompanyInfo = await _context.Set<Company>().Where(C=> C.CompanyId == CompanyId).FirstOrDefaultAsync();
                string Website =await LookupWithKey("DOMAIN");
                string Portal = await LookupWithKey("PORTAL");
                string ValdiateURL = await LookupWithKey("EMAIL_VALIDATE_URL");
                string hostname = await LookupWithKey("SMTPHOST");
                string fromadd = await LookupWithKey("EMAILFROM");
                string sso_login = await _companyRepository.GetCompanyParameter("AAD_SSO_TENANT_ID", CompanyId);

                string Verifylink = Portal + ValdiateURL + CompanyId + "/" + guid;
                string CompanyLogo = Portal + "/uploads/" + CompanyInfo.CompanyId + "/companylogos/" + CompanyInfo.CompanyLogoPath;

                if (string.IsNullOrEmpty(CompanyInfo.CompanyLogoPath))
                {
                    CompanyLogo = await LookupWithKey("CCLOGO");
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
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", CompanyInfo.CustomerId);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                    messagebody = messagebody.Replace("{VERIFY_LINK}", Verifylink);
                    messagebody = messagebody.Replace("{CC_WEBSITE}", Website);
                    messagebody = messagebody.Replace("{PORTAL}", Portal);

                    string[] toEmails = { EmailId };

                    bool ismailsend = await _senderEmail.Email(toEmails, messagebody, fromadd, hostname, Subject);
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public async Task UpdateTemp(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User Profile has been updated {user.UserId}");
          
        }
        public async Task<List<Sectors>> GetSectors()
        {
            try
            {
                var result = await _context.Set<Sectors>().FromSqlRaw("Pro_Get_IndustrySector").ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<PackageModel>> GetAllPackagePlan()
        {
            try
            {
                var packageInfo = await (from PP in _context.Set<PackagePlan>()
                                         where PP.Status == 1
                                         select new PackageModel()
                                         {
                                             PackagePlanId = PP.PackagePlanId,
                                             PackageName = PP.PlanName,
                                             PackageDescription = PP.PlanDescription,
                                         }).ToListAsync();

                return packageInfo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateCompanyStatus(ViewCompanyModel companyModel)
        {
            var CompanyStatus = await (from C in _context.Set<Company>() where C.CompanyId == companyModel.CompanyId select C).FirstOrDefaultAsync();
            if (CompanyStatus != null)
            {
                CompanyStatus.CompanyProfile = companyModel.CompanyProfile;
                CompanyStatus.OnTrial = _DBC.OnTrialStatus(companyModel.CompanyProfile, CompanyStatus.OnTrial);
                CompanyStatus.Status = companyModel.Status;
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new CompanyNotFoundException(CompanyId, UserID);
            }
        }

        public async Task<int> CreateCompany(string companyName, int status, string companyProfile, string isd, string switshBoardPhone, DateTime registrationDate,
            DateTime anniversary, int packagePlanId, int timeZone, string customerId, string invitationCode, string sector)
        {
            Company TblComp = new Company();
            TblComp.CompanyName = companyName;
            TblComp.Status = status;
            TblComp.CompanyProfile = companyProfile;
            TblComp.OnTrial = _DBC.OnTrialStatus(companyProfile, false);
            TblComp.Isdcode = isd;
            TblComp.SwitchBoardPhone = switshBoardPhone;
            TblComp.RegistrationDate = registrationDate;
            TblComp.AnniversaryDate = anniversary;
            TblComp.CompanyLogoPath = "";
            TblComp.PackagePlanId = packagePlanId;
            TblComp.CreatedOn = _DBC.GetDateTimeOffset(DateTime.Now);
            TblComp.UpdatedOn = _DBC.GetDateTimeOffset(DateTime.Now);
            TblComp.TimeZone = timeZone;
            TblComp.CreatedBy = 0;
            TblComp.UpdatedBy = 0;
            TblComp.UniqueKey = Guid.NewGuid().ToString();
            TblComp.CustomerId = customerId;
            TblComp.InvitationCode = invitationCode;
            TblComp.Sector = sector;
            TblComp.ContactLogoPath = "";
            _context.Set<Company>().Add(TblComp);
            await _context.SaveChangesAsync();
            return TblComp.CompanyId;
        }

        public async Task<RegRet> CreateRegistrationData(int regId, int companyId, int userId, string lat, string lng)
        {
            try
            {
                DateTimeOffset CreatedNow = _DBC.GetDateTimeOffset(DateTime.Now);

                var pRegID = new SqlParameter("@RegistrationID", regId);
                var pCompanyID = new SqlParameter("@CompanyID", companyId);
                var pUserID = new SqlParameter("@UserID", userId);
                var pCreatedNow = new SqlParameter("@CreatedOnOffset", CreatedNow);
                var pLat = new SqlParameter("@LocationLat", lat);
                var pLng = new SqlParameter("@LocationLng", lng);
                var err = await _context.Set<RegRet>().FromSqlRaw("EXEC Pro_Registration_Data_Create @RegistrationID, @CompanyID, @UserID, @CreatedOnOffset,@LocationLat,@LocationLng",
                    pRegID, pCompanyID, pUserID, pCreatedNow, pLat, pLng).FirstOrDefaultAsync();
                return err;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async void SetupCompanyPaymentProfile(int companyId, int userId, int packagePlanId, string timeZoneId)
        {
            var comp_profile = await (from CP in _context.Set<CompanyPaymentProfile>() where CP.CompanyId == companyId select CP).AnyAsync();
            if (comp_profile)
            {
                var PaymentProfile = await (from PP in _context.Set<PaymentProfile>() select PP).FirstOrDefaultAsync();
                
                decimal VatRate = Convert.ToDecimal(_DBC.LookupWithKey("COMP_VAT"));

                if (packagePlanId == 1 && PaymentProfile.CreditBalance > 0)
                {

                    var topup_type = await (from TT in _context.Set<TransactionType>()
                                      where TT.TransactionCode == "TOPUP"
                                      select TT).FirstOrDefaultAsync();

                    VatRate = (VatRate + 100) / 100;
                    UpdateTransactionDetailsModel IP = new UpdateTransactionDetailsModel();
                    IP.CustomerId = companyId;
                    IP.TransactionTypeId = topup_type.TransactionTypeId;
                    IP.TransactionRate = PaymentProfile.CreditBalance;
                    IP.MinimumPrice = PaymentProfile.CreditBalance;
                    IP.Quantity = 1;
                    IP.Cost = PaymentProfile.CreditBalance;
                    IP.LineValue = PaymentProfile.CreditBalance;
                    IP.Vat = PaymentProfile.CreditBalance * 0;
                    IP.Total = PaymentProfile.CreditBalance;
                    IP.TransactionDate = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                    IP.TransactionReference = "FREE CREDIT";
                    IP.TransactionDetailsId = 0;
                    IP.TransactionStatus = 1;
                    IP.Storage = 0;
                    IP.DRCR = "CR";

                    //ADM.AddTransaction(IP, UserID, companyId, timeZoneId);
                }
            }
        }

        public async Task<UserValidatedDTO> CompleteRegistration(TempRegister tempRegister)
        {
            //int CompanyID = 0;
            //string CustomerId = string.Empty;
            //try
            //{
            //    UserValidatedDTO UserDTO = new UserValidatedDTO();
            //    var reg = await (from R in _context.Set<Registration>() where R.Id == tempRegister.RegId select R).FirstOrDefaultAsync();
            //    if (reg != null)
            //    {
            //        using (TransactionScope scope = new TransactionScope())
            //        {
            //            CustomerId = reg.CustomerId;

            //            string InvitationCode = _DBC.Left(Guid.NewGuid().ToString().Replace("-", ""), 8).ToUpper();
            //            CompanyID = await CreateCompany(reg.CompanyName, 2, "AWAITING_SETUP", reg.MobileIsd, reg.MobileNo, DateTime.Now, DateTime.Now.AddMonths(1), (int)reg.PackagePlanId, 26, IP.CustomerId, InvitationCode, reg.Sector);

            //            if (CompanyID > 0)
            //            {

            //                string UniqueId = Guid.NewGuid().ToString();
            //                int NewUserId = UH.CreateUsers(CompanyID, true, reg.FirstName, reg.Email, reg.Password, 1, 0,
            //                    "GMT Standard Time", reg.LastName, reg.MobileNo, "SUPERADMIN", "", reg.MobileIsd, "", "", "",
            //                    UniqueId, "", "", "", true, "en");


            //                if (NewUserId > 0)
            //                {

            //                    var comp =await _context.Set<Company>().Where(w => w.CompanyId == CompanyID).FirstOrDefaultAsync();
            //                    comp.CreatedBy = NewUserId;
            //                    comp.UpdatedBy = NewUserId;

            //                    var UserInfo = await (from User in _context.Set<User>() where User.UserId == NewUserId select User).FirstOrDefaultAsync();
            //                    UserInfo.FirstLogin = false;
            //                    await _context.SaveChangesAsync();

            //                    //Send the activation email link
            //                    string Plan = await _context.Set<PackagePlan>().Where(w => w.PackagePlanId == reg.PackagePlanId).Select(s => s.PlanName).FirstOrDefaultAsync();

            //                    string TimeZoneId = _DBC.GetTimeZoneByCompany(CompanyID);

            //                    //Create default location "ALL" and assgin to user to it.
            //                    LatLng LL = _DBC.GetCoordinates(reg.City + ", " + reg.Postcode);
            //                    try
            //                    {
            //                        RegRet err = await CreateRegistrationData(reg.Id, CompanyID, NewUserId, LL.Lat, LL.Lng);
            //                        if (err.Error != 0)
            //                        {
            //                            throw new Exception("Company Data not created");
            //                        }
            //                        else
            //                        {

            //                            //Setup company payment profile
            //                            SetupCompanyPaymentProfile(CompanyID, NewUserId, (int)reg.PackagePlanId, TimeZoneId);

            //                            //Add license item in the company transaction type table.
            //                            //DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //                            DateTime NextRun = DateTime.Now.AddDays(30);
            //                            var transac_type = (from TT in _context.Set<TransactionType>()
            //                                                where TT.TransactionCode == "LICENSEFEE"
            //                                                select TT).FirstOrDefault();

            //                            int id = _billing.UpdateCompanyTranscationType(CompanyID, NewUserId, TimeZoneId, transac_type.TransactionTypeID, transac_type.Rate, 0, "MONTHLY", NextRun);

            //                            _context.Set<Registration>().Remove(reg);
            //                            _context.SaveChanges();

            //                            //Confirm the account and send the account details to the user.
            //                            _SDE.CompanySignUpConfirm(reg.Email, reg.FirstName + " " + reg.LastName, reg.MobileISD + reg.MobileNo, reg.PaymentMethod, Plan, reg.Password, CompanyID);

            //                            UserDTO.companyid = CompanyID;
            //                            UserDTO.userid = NewUserId;
            //                            UserDTO.Password = reg.Password;
            //                            UserDTO.CustomerId = reg.CustomerId;
            //                            UserDTO.Token = UniqueId;
            //                            UserDTO.ErrorId = 0;
            //                            UserDTO.Message = "Validated Sucessfully";

            //                            string companyname = string.IsNullOrEmpty(reg.CompanyName) ? "New Company " + CompanyID : reg.CompanyName;
            //                            CustomerId = string.IsNullOrEmpty(reg.CustomerId) ? "newcompany" + CompanyID : reg.CustomerId;

            //                            try
            //                            {
            //                                InsertCompanyApi(CompanyID, companyname, reg.CustomerId, InvitationCode);
            //                            }
            //                            catch (Exception ex)
            //                            {
            //                                DBC.catchException(ex);
            //                                DeleteCompanyApi(CompanyID, CustomerId);
            //                            }
            //                        }
            //                        scope.Complete();
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        DBC.catchException(ex);
            //                    }
            //                    return UserDTO;
            //                }
            //                else
            //                {
            //                    ResultDTO.ErrorId = 100;
            //                    ResultDTO.Message = "User not created";
            //                }
            //            }
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{

            //    ResultDTO = DBC.catchException(ex);
            //    DeleteCompanyApi(CompanyID, CustomerId);
            //}
            //return ResultDTO;
            throw new NotImplementedException();
        }

        public Task<bool> DeleteTempRegistration(TempRegister tempRegister)
        {
            throw new NotImplementedException();
        }

        public Task<List<Sectors>> BusinessSector()
        {
            throw new NotImplementedException();
        }
    }
    
}
