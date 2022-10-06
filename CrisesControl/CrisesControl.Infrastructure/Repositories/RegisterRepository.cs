
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Billing.Repositories;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Models;
using CrisesControl.Core.Register;
using CrisesControl.Core.Register.Repositories;
using CrisesControl.Core.Users;
using CrisesControl.Core.Users.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using CrisesControl.SharedKernel.Enums;
using CrisesControl.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
        private readonly IDBCommonRepository _DBC;
        private readonly ISenderEmailService _SDE;
        private readonly BillingHelper _billingHelper;

        private int UserID;
        private int CompanyId;
        public RegisterRepository(  
            CrisesControlContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<RegisterRepository> logger,
            ISenderEmailService SDE, IDBCommonRepository DBC
            )
        {
           this._logger = logger;
          this._context = context;
          this._httpContextAccessor = httpContextAccessor;
          this._DBC =DBC;
            this._SDE = SDE;
            this._billingHelper = new BillingHelper(_context,_DBC,_SDE);
          
        }
        public async Task<List<Registration>> GetAllRegistrations()
        {
           var registrations=  await _context.Set<Registration>().ToListAsync();
            return registrations;
        }
       public async Task<bool> CheckCustomer(string customerId)
        {
            try
            {
                customerId = customerId.Trim().ToLower();
                var Customer = await _context.Set<Company>()
                                     .Include(c=>c.StdTimeZone)
                                     .Where(C => C.CustomerId == customerId)
                                     .AnyAsync();
                return Customer;
            }
            catch (Exception ex)
            {
               _logger.LogError("Error occured while seeding into database {0}, {1}",ex.Message,ex.InnerException);
                return false;
            }
        }

        public async Task<CommsStatus> SendText(string isd, string toNumber, string message, string callbackUrl = "")
        {
            try
            {
                CommsStatus textrslt = new CommsStatus();
                string TwilioRoutingApi = string.Empty;
                string FromNumber =await _DBC.LookupWithKey(KeyType.TWILIOFROMNUMBER.ToDbKeyString());

                bool SendInDirect =await _DBC.IsTrue(await _DBC.LookupWithKey(KeyType.TWILIOUSEINDIRECTCONNECTION.ToDbKeyString()), false);

                TwilioRoutingApi =await _DBC.LookupWithKey(KeyType.TWILIOROUTINGAPI.ToDbKeyString());

                var Confs = await  _context.Set<SysParameter>().Where(L=> L.Name ==KeyType.USEMESSAGINGCOPILOT.ToDbKeyString() || L.Name == KeyType.MESSAGINGCOPILOTSID.ToDbKeyString()).ToListAsync();

                bool USE_MESSAGING_COPILOT = Convert.ToBoolean(Confs.Where(w => w.Name == "USE_MESSAGING_COPILOT").Select(s => s.Value).FirstOrDefault());
                string MESSAGING_COPILOT_SID =Confs.Where(w => w.Name == "MESSAGING_COPILOT_SID").Select(s => s.Value).FirstOrDefault().ToString();

                dynamic CommsAPI = _DBC.InitComms("TWILIO");
                CommsAPI.USE_MESSAGING_COPILOT = USE_MESSAGING_COPILOT;
                CommsAPI.MESSAGING_COPILOT_SID = MESSAGING_COPILOT_SID;
                CommsAPI.SendInDirect = SendInDirect;
                CommsAPI.TwilioRoutingApi = TwilioRoutingApi;

                string ClMessageId = string.Empty;
                string istextsend = "NOTSENT";

                //Getting the from number based on the destination.
                var FromNum = await _context.Set<PhoneNumberMapping>().Where(F=> F.CountryDialCode == isd).FirstOrDefaultAsync();
                if (FromNum != null)
                {
                    FromNumber =  FromNum.FromNumber;
                }

                toNumber = toNumber.FixMobileZero();

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

                return new CommsStatus
                {
                    CurrentAction = "Error"
                };
            }
        }

        public async Task<string> ValidateMobile(string code, string isd, string mobileNo, string message = "")
        {
            try
            {

                string validation_method = await _DBC.LookupWithKey(KeyType.PHONEVALIDATIONMETHOD.ToDbKeyString());
                bool SendInDirect =await _DBC.IsTrue(await _DBC.LookupWithKey(KeyType.TWILIOUSEINDIRECTCONNECTION.ToDbKeyString()), false);
                string TwilioRoutingApi = await _DBC.LookupWithKey(KeyType.TWILIOROUTINGAPI.ToDbKeyString());

                if (string.IsNullOrEmpty(message))
                    message = await _DBC.LookupWithKey(KeyType.PHONEVALIDATIONMSG.ToDbKeyString());

                message = message.Replace("{OTP}", code).Replace("{CODE}", code);

                mobileNo = mobileNo.FixMobileZero();

                if (validation_method == MessageType.Text.ToDbString())
                {
                    CommsStatus status = await SendText(isd, mobileNo, message);
                    return status.CurrentAction;
                }
                else
                {
                    CommsStatus status =await VerificationCall(mobileNo, message, SendInDirect, TwilioRoutingApi);
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

                string FromNumber = await _DBC.LookupWithKey("TWILIO_FROM_NUMBER");
                string MsgXML = await _DBC.LookupWithKey("TWILIO_PHONE_VALIDATION_XML");
                // TODO: still going to investigate this
               // MsgXML = MsgXML + "?Body=" + _httpContextAccessor.HttpContext.Server.UrlEncode(message);

                dynamic CommsAPI = _DBC.InitComms("TWILIO");
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

        //public async Task<dynamic> InitComms(string API_CLASS, string APIClass = "", string clientId = "", string clientSecret = "")
        //{
            
        //    try
        //    {

        //        int RetryCount = 2;
        //        int.TryParse( _DBC.LookupWithKey(API_CLASS + KeyType.MESSAGERETRYCOUNT.ToDbKeyString()), out RetryCount);

        //        if (string.IsNullOrEmpty(APIClass))
        //            APIClass = _DBC.LookupWithKey(API_CLASS + KeyType.APICLASS.ToDbKeyString());

        //        if (string.IsNullOrEmpty(clientId))
        //            clientId = _DBC.LookupWithKey(API_CLASS + KeyType._CLIENTID.ToDbKeyString());

        //        if (string.IsNullOrEmpty(clientSecret))
        //            clientSecret = _DBC.LookupWithKey(API_CLASS + KeyType.CLIENTSECRET.ToDbKeyString());

        //        string[] TmpClass = APIClass.Trim().Split('|');

        //        string binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin");

        //        Assembly assembly = Assembly.LoadFrom(binPath + "\\" + TmpClass[0]);
        //        Type type = assembly.GetType(TmpClass[1]);
        //        dynamic CommsAPI = Activator.CreateInstance(type);

        //        CommsAPI.ClientId = clientId;
        //        CommsAPI.Secret = clientSecret;
        //        CommsAPI.RetryCount = RetryCount;

        //        return CommsAPI;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //        return null;
        //    }
        //}

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
        public async Task CreateObjectRelationship(int targetObjectId, int sourceObjectId, string relationName, int companyId, int createdUpdatedBy, string timeZoneId, string relatinFilter = "")
        {
            try
            {
              

                if (relationName.ToUpper() == "GROUP" || relationName.ToUpper() == "LOCATION")
                {
                    if (targetObjectId > 0)
                    {
                        int NewSourceObjectID = 0;

                        var ObjMapId = await _context.Set<ObjectMapping>().Include(om => om.Object).Where(OBJ => OBJ.Object.ObjectName == relationName).Select(a => a.ObjectMappingId).FirstOrDefaultAsync();

                        if (sourceObjectId > 0)
                        {
                            NewSourceObjectID = sourceObjectId;
                            await CreateNewObjectRelation(NewSourceObjectID, targetObjectId, ObjMapId, createdUpdatedBy, timeZoneId);
                        }

                        if (!string.IsNullOrEmpty(relatinFilter))
                        {
                            if (relationName.ToUpper() == "GROUP")
                            {
                                NewSourceObjectID = await _context.Set<Group>().Where(D=> D.GroupName == relatinFilter && D.CompanyId == CompanyId).Select(g=>g.GroupId).FirstOrDefaultAsync();
                            }
                            else if (relationName.ToUpper() == "LOCATION")
                            {
                                NewSourceObjectID = await  _context.Set<Location>().Where(L=> L.LocationName == relatinFilter && L.CompanyId == CompanyId).Select(L=>L.LocationId).FirstOrDefaultAsync();
                            }
                            await CreateNewObjectRelation(NewSourceObjectID, targetObjectId, ObjMapId, createdUpdatedBy, timeZoneId);
                        }
                    }
                }
                else if (relationName.ToUpper() == "DEPARTMENT")
                {
                  await  UpdateUserDepartment(sourceObjectId, createdUpdatedBy, timeZoneId);
                }
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public async Task CreateNewObjectRelation(int sourceObjectId, int targetObjectId, int objMapId, int createdUpdatedBy, string timeZoneId)
        {
            try
            {

                bool IsALLOBJrelationExist = await _context.Set<ObjectRelation>().Where(OBR=> OBR.TargetObjectPrimaryId == targetObjectId
                                              && OBR.ObjectMappingId == objMapId
                                              && OBR.SourceObjectPrimaryId == sourceObjectId).AnyAsync();
                if (!IsALLOBJrelationExist)
                {
                    ObjectRelation tblDepObjRel = new ObjectRelation()
                    {
                        TargetObjectPrimaryId = targetObjectId,
                        ObjectMappingId = objMapId,
                        SourceObjectPrimaryId = sourceObjectId,
                        CreatedBy = createdUpdatedBy,
                        UpdatedBy = createdUpdatedBy,
                        CreatedOn = System.DateTime.Now,
                        UpdatedOn = CrisesControl.SharedKernel.Utils.DateTimeExtensions.GetLocalTime(timeZoneId, System.DateTime.Now),
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
        public async Task UpdateUserDepartment(int departmentId, int createdUpdatedBy, string timeZoneId)
        {
            try
            {
                UserID =Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst("sub"));
                var user = await _context.Set<User>().Where(U=> U.UserId == UserID).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.DepartmentId = departmentId;
                    user.UpdatedBy = createdUpdatedBy;
                    user.UpdatedOn = DateTime.Now.GetDateTimeOffset(timeZoneId);
                   await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task NewUserAccountConfirm(string emailId, string userName, string userPass, int companyId, string guid)
        {
            try
            {

                //string path = Convert.ToString(DBC.LookupWithKey("API_TEMPLATE_PATH")) + "NewAccountConfirmed.html";

                
                string Subject = string.Empty;
                string message = Convert.ToString(_DBC.ReadHtmlFile("NEW_ACCOUNT_CONFIRMED", "DB", CompanyId,   Subject));

                var CompanyInfo =await  _context.Set<Company>().Where(C=> C.CompanyId == CompanyId ).FirstOrDefaultAsync();
                string Website = await _DBC.LookupWithKey("DOMAIN");
                string Portal = await _DBC.LookupWithKey("PORTAL");
                string ValdiateURL =await _DBC.LookupWithKey("EMAIL_VALIDATE_URL");
                string hostname =await _DBC.LookupWithKey("SMTPHOST");
                string fromadd =await _DBC.LookupWithKey("EMAILFROM");
                string sso_login =await _DBC.GetCompanyParameter("AAD_SSO_TENANT_ID", CompanyId);

                string Verifylink = Portal + ValdiateURL + CompanyId + "/" + guid;
                string CompanyLogo = Portal + "/uploads/" + CompanyInfo.CompanyId + "/companylogos/" + CompanyInfo.CompanyLogoPath;

                if (string.IsNullOrEmpty(CompanyInfo.CompanyLogoPath))
                {
                    CompanyLogo = await _DBC.LookupWithKey("CCLOGO");
                }

                if (!string.IsNullOrEmpty(sso_login))
                    userPass = "Use the single sign-on to login";

                if ((message != null) && (hostname != null) && (fromadd != null))
                {
                    string messagebody = message;

                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", userName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", emailId);
                    messagebody = messagebody.Replace("{RECIPIENT_PASSWORD}", userPass);
                    messagebody = messagebody.Replace("{COMPANY_NAME}", CompanyInfo.CompanyName);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                    messagebody = messagebody.Replace("{VERIFY_LINK}", Verifylink);
                    messagebody = messagebody.Replace("{CC_WEBSITE}", Website);
                    messagebody = messagebody.Replace("{PORTAL}", Portal);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", CompanyInfo.CustomerId);

                    messagebody = messagebody.Replace("{TWITTER_LINK}",await _DBC.LookupWithKey("CC_TWITTER_PAGE"));
                    messagebody = messagebody.Replace("{TWITTER_ICON}",await _DBC.LookupWithKey("CC_TWITTER_ICON"));
                    messagebody = messagebody.Replace("{FACEBOOK_LINK}",await _DBC.LookupWithKey("CC_FB_PAGE"));
                    messagebody = messagebody.Replace("{FACEBOOK_ICON}",await _DBC.LookupWithKey("CC_FB_ICON"));
                    messagebody = messagebody.Replace("{LINKEDIN_LINK}",await _DBC.LookupWithKey("CC_LINKEDIN_PAGE"));
                    messagebody = messagebody.Replace("{LINKEDIN_ICON}",await _DBC.LookupWithKey("CC_LINKEDIN_ICON"));

                    string[] toEmails = { emailId };

                    bool ismailsend =await _SDE.Email(toEmails, messagebody, fromadd, hostname, Subject);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public  StringBuilder ReadHtmlFile(string fileCode, string source, int companyId, out string subject, string provider = "AWSSES")
        {
            StringBuilder htmlContent = new StringBuilder();
            string line;
            subject = "";
            try
            {
                if (source == "FILE")
                {
                    using (StreamReader htmlReader = new System.IO.StreamReader(fileCode))
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
                                .Where(MSG => MSG.CompanyId == 0 && MSG.Code == fileCode)
                                .Union(_context.Set<EmailTemplate>()
                                .Where(MSG => MSG.Code == fileCode && MSG.CompanyId == CompanyId))
                                .OrderByDescending(MSG => MSG.CompanyId).FirstOrDefault();
                    {
                        subject = content.EmailSubject;

                        var head =  _context.Set<EmailTemplate>().Where(MSG => MSG.Code == "DOCHEAD").FirstOrDefault();
                        if (head != null)
                        {
                            htmlContent.AppendLine(head.HtmlData.ToString());
                        }

                        htmlContent.Append(content.HtmlData.ToString());

                        if (provider.ToUpper() != "OFFICE365")
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
        public async Task<bool> UpgradeRequest(int companyId)
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
                string Website =await _DBC.LookupWithKey("DOMAIN");
                string Portal =await _DBC.LookupWithKey("PORTAL");
                string AdminPortal =await _DBC.LookupWithKey("ADMIN_SITE_URL");
                string hostname =await _DBC.LookupWithKey("SMTPHOST");
                string fromadd =await _DBC.LookupWithKey("EMAILFROM");

                string message = Convert.ToString(await _DBC.ReadHtmlFile(template, "DB", CompanyId,   Subject));

                if ((hostname != null) && (fromadd != null))
                {

                    StringBuilder adminMsg = new StringBuilder();

                    adminMsg.AppendLine("<h2>An upgrade request has been made by customer</h2>");
                    adminMsg.AppendLine("<p>Below are the company information:</p>");
                    adminMsg.AppendLine("<strong>Company ID: </strong>CC" + Company.CompanyId + "</br>");
                    adminMsg.AppendLine("<strong>Company Name: </strong>" + Company.CompanyName + "</br>");
                    adminMsg.AppendLine("<strong>Registered On: </strong>" + Company.RegistrationDate.ToString("dd-MM-yy HH:mm") + "</br>");

                    adminMsg.AppendLine("<p style=\"color:#ff0000;font-size:18px\"><strong><a href=\"" + AdminPortal + "\">Click here to login to admin portal to view company details</a></p>");

                    string[] AdminEmail = (await _DBC.LookupWithKey("BILLING_EMAIL")).Split(',');

                    await _SDE.Email(AdminEmail, adminMsg.ToString(), fromadd, hostname, "Crises Control: " + Company.CompanyName + " requsted for an upgrade");


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
        public async Task<Registration> GetRegistrationByUniqueReference(string uniqueRef)
        {
            var reg = await _context.Set<Registration>().Where(R => R.UniqueReference == uniqueRef ).FirstOrDefaultAsync();
            return reg;
        }
        public async Task<Registration> GetRegistrationDataByEmail(string email)
        {
            var reg = await _context.Set<Registration>().Where(R =>  R.Email == email).OrderBy(a => a.Id).FirstOrDefaultAsync();
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
        public async Task<Registration> GetTempRegistration(int regId, string uniqueRef)
        {
            try
            {
                if (regId > 0)
                {
                    var reg = await _context.Set<Registration>().Where(R=> R.Id == regId).FirstOrDefaultAsync();
                    return reg;
                }
                else if (!string.IsNullOrEmpty(uniqueRef))
                {
                    var reg = await _context.Set<Registration>().Where(R=> R.UniqueReference == uniqueRef).FirstOrDefaultAsync();
                    return reg;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while seeding into database {0}, {1},{2}",ex.Message,ex.InnerException,ex.StackTrace);
              
            }
            throw new RegisterNotFoundException(regId, 0);


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

        public async Task<bool> ActivateCompany(int userId, string activationKey, string ipAddress, int salesSource = 0, string timeZoneId = "GMT Standard Time")
        {

            try
            {
                int getuser = await _context.Set<User>().Where(U=> U.UserId == userId).Select(U=>U.CompanyId).FirstOrDefaultAsync();

                if (getuser > 0)
                {
                    var checkkey =  _context.Set<Company>().Include(CP=>CP.CompanyPaymentProfiles)
                                    .Include(CA=>CA.CompanyActivation)                                   
                                    .Where(CA=> CA.CompanyActivation.ActivationKey == activationKey && CA.CompanyId == getuser && CA.Status == 0
                                    ).FirstOrDefault();
                    if (checkkey != null)
                    {
                        checkkey.CompanyActivation.ActivatedBy = userId;
                        checkkey.CompanyActivation.ActivatedOn = DateTime.Now.GetDateTimeOffset( timeZoneId);

                        if (checkkey.Status == 4)
                        {
                            checkkey.CompanyProfile = "AWAITING_SETUP";
                        }

                        checkkey.CompanyActivation.Status = 1;
                        checkkey.CompanyActivation.Ipaddress = ipAddress;
                        if (checkkey.CompanyActivation.SalesSource <= 0)
                            checkkey.CompanyActivation.SalesSource = salesSource;
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
                throw new CompanyNotFoundException(CompanyId, userId);
                return false;
            }
        }
        public async Task<UserDevice> GetUserDeviceByUserId(int userId)
        {
            var device =await  _context.Set<UserDevice>().Where(UD=> UD.UserId == userId && UD.Status == 1 ).FirstOrDefaultAsync();
            return device;
  
        }
        public async Task<User> GetUserByUniqueId(string uniqueId)
        {
            var data = await _context.Set<User>().Where(U => U.UniqueGuiId == uniqueId)
                      .FirstOrDefaultAsync();
            return data;

        }
        public async Task<CompanyUser> SendVerification(string uniqueId)
        {
            var data = await _context.Set<User>().Include(x=>x.Company).Where(U=>U.UniqueGuiId == uniqueId)
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
        public async Task NewUserAccount(string emailId, string userName, int companyId, string guid)
        {
            try
            {
                //string path = Convert.ToString(DBC.LookupWithKey("API_TEMPLATE_PATH")) + "NewUserAccount.html";
                string Subject = string.Empty;

                string message = Convert.ToString(ReadHtmlFile("NEW_USER_ACCOUNT", "DB", companyId, out Subject));
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

                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", userName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", emailId);
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

                    string[] toEmails = { emailId };
                    bool ismailsend =await _SDE.Email(toEmails, messagebody, fromadd, hostname, Subject);
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
        public async Task SendCredentials(string emailId, string userName, string userPass, int companyId, string guid)
        {
            try
            {
                string Subject = string.Empty;
                string message = Convert.ToString(ReadHtmlFile("SEND_CREDENTIAL", "DB", CompanyId, out Subject));

                var CompanyInfo = await _context.Set<Company>().Where(C=> C.CompanyId == CompanyId).FirstOrDefaultAsync();
                string Website =await _DBC.LookupWithKey("DOMAIN");
                string Portal =await _DBC.LookupWithKey("PORTAL");
                string ValdiateURL =await _DBC.LookupWithKey("EMAIL_VALIDATE_URL");
                string hostname =await _DBC.LookupWithKey("SMTPHOST");
                string fromadd =await _DBC.LookupWithKey("EMAILFROM");
                string sso_login =await _DBC.GetCompanyParameter("AAD_SSO_TENANT_ID", CompanyId);

                string Verifylink = Portal + ValdiateURL + CompanyId + "/" + guid;
                string CompanyLogo = Portal + "/uploads/" + CompanyInfo.CompanyId + "/companylogos/" + CompanyInfo.CompanyLogoPath;

                if (string.IsNullOrEmpty(CompanyInfo.CompanyLogoPath))
                {
                    CompanyLogo = await _DBC.LookupWithKey("CCLOGO");
                }

                if (!string.IsNullOrEmpty(sso_login))
                    userPass = "Use the single sign-on to login";

                if ((message != null) && (hostname != null) && (fromadd != null))
                {
                    string messagebody = message;

                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", userName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", emailId);
                    messagebody = messagebody.Replace("{RECIPIENT_PASSWORD}", userPass);
                    messagebody = messagebody.Replace("{COMPANY_NAME}", CompanyInfo.CompanyName);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", CompanyInfo.CustomerId);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                    messagebody = messagebody.Replace("{VERIFY_LINK}", Verifylink);
                    messagebody = messagebody.Replace("{CC_WEBSITE}", Website);
                    messagebody = messagebody.Replace("{PORTAL}", Portal);

                    string[] toEmails = { emailId };

                    bool ismailsend = await _SDE.Email(toEmails, messagebody, fromadd, hostname, Subject);
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
                var result = await _context.Set<Sectors>().FromSqlRaw("exec Pro_Get_IndustrySector").ToListAsync();
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
                var packageInfo = await  _context.Set<PackagePlan>()
                                         .Where(PP=> PP.Status == 1)
                                         .Select(PP => new PackageModel()
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
                CompanyStatus.OnTrial = await _DBC.OnTrialStatus(companyModel.CompanyProfile, CompanyStatus.OnTrial);
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
            TblComp.OnTrial = await _DBC.OnTrialStatus(companyProfile, false);
            TblComp.Isdcode = isd;
            TblComp.SwitchBoardPhone = switshBoardPhone;
            TblComp.RegistrationDate = registrationDate;
            TblComp.AnniversaryDate = anniversary;
            TblComp.CompanyLogoPath = "";
            TblComp.PackagePlanId = packagePlanId;
            TblComp.CreatedOn = await _DBC.GetDateTimeOffset(DateTime.Now);
            TblComp.UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now);
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
                DateTimeOffset CreatedNow = await _DBC.GetDateTimeOffset(DateTime.Now);

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
                    IP.TransactionDate = await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                    IP.TransactionReference = "FREE CREDIT";
                    IP.TransactionDetailsId = 0;
                    IP.TransactionStatus = 1;
                    IP.Storage = 0;
                    IP.DRCR = "CR";

                   await  _billingHelper.AddTransaction(IP, UserID, companyId, timeZoneId);
                }
            }
        }

        public async Task<bool> InsertCompanyApi(int companyId, string companyName, string customerId, string invitationCode)
        {
            try
            {
                string CompanyApi = await _DBC.LookupWithKey("COMPANY_API_MGMT_URL");
                string Host =await _DBC.LookupWithKey("CRISES_CONTROL_HOST");
                string ApiMode =await _DBC.LookupWithKey("CRISES_CONTROL_API_MODE");

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(CompanyApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var cmpapirequest = new CompanyApiRequest()
                {
                    ApiHost = Host,
                    CompanyId = CompanyId,
                    CompanyName = companyName,
                    CustomerId = customerId,
                    InvitationCode = invitationCode,
                    ApiMode = ApiMode
                };

                HttpResponseMessage RspApi = client.PostAsJsonAsync("Company/SaveCompanyApi", cmpapirequest).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result.Trim();
                if (RspApi.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCompanyApi(int companyId, string customerId)
        {
            try
            {
                string CompanyApi = await _DBC.LookupWithKey("COMPANY_API_MGMT_URL");

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(CompanyApi);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var cmpapirequest = new CompanyApiRequest()
                {
                    ApiHost = "",
                    CompanyId = CompanyId,
                    CompanyName = "",
                    CustomerId = customerId,
                    InvitationCode = "",
                    ApiMode = ""
                };

                HttpResponseMessage RspApi = client.PostAsJsonAsync("Company/DeleteCompanyApi", cmpapirequest).Result;
                Task<string> resultstring = RspApi.Content.ReadAsStringAsync();
                string ressultstr = resultstring.Result.Trim();
                if (RspApi.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<UserValidatedDTO> CompleteRegistration(TempRegister tempRegister)
        {
            int CompanyID = 0;
            string CustomerId = string.Empty;
            try
            {
                UserValidatedDTO UserDTO = new UserValidatedDTO();
                var reg = await (from R in _context.Set<Registration>() where R.Id == tempRegister.RegId select R).FirstOrDefaultAsync();
                if (reg != null)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        CustomerId = reg.CustomerId;

                        string InvitationCode = _DBC.Left(Guid.NewGuid().ToString().Replace("-", ""), 8).ToUpper();
                        CompanyID = await CreateCompany(reg.CompanyName, 2, "AWAITING_SETUP", reg.MobileIsd, reg.MobileNo, DateTime.Now, DateTime.Now.AddMonths(1), (int)reg.PackagePlanId, 26, tempRegister.CustomerId, InvitationCode, reg.Sector);

                        if (CompanyID > 0)
                        {

                            string UniqueId = Guid.NewGuid().ToString();
                            int NewUserId =  await CreateUsers(CompanyID, true, reg.FirstName, reg.Email, reg.Password, 1, 0,
                                "GMT Standard Time", reg.LastName, reg.MobileNo, "SUPERADMIN", "", reg.MobileIsd, "", "", "",
                                UniqueId, "", "", "", true, "en");


                            if (NewUserId > 0)
                            {

                                var comp = await _context.Set<Company>().Where(w => w.CompanyId == CompanyID).FirstOrDefaultAsync();
                                comp.CreatedBy = NewUserId;
                                comp.UpdatedBy = NewUserId;

                                var UserInfo = await (from User in _context.Set<User>() where User.UserId == NewUserId select User).FirstOrDefaultAsync();
                                UserInfo.FirstLogin = false;
                                await _context.SaveChangesAsync();

                                //Send the activation email link
                                string Plan = await _context.Set<PackagePlan>().Where(w => w.PackagePlanId == reg.PackagePlanId).Select(s => s.PlanName).FirstOrDefaultAsync();

                                string TimeZoneId = await _DBC.GetTimeZoneByCompany(CompanyID);

                                //Create default location "ALL" and assgin to user to it.
                                LatLng LL = await _DBC.GetCoordinates(reg.City + ", " + reg.Postcode);
                                try
                                {
                                    RegRet err = await CreateRegistrationData(reg.Id, CompanyID, NewUserId, LL.Lat, LL.Lng);
                                    if (err.Error != 0)
                                    {
                                        throw new Exception("Company Data not created");
                                    }
                                    else
                                    {

                                        //Setup company payment profile
                                        SetupCompanyPaymentProfile(CompanyID, NewUserId, (int)reg.PackagePlanId, TimeZoneId);

                                        //Add license item in the company transaction type table.
                                        //DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                                        DateTime NextRun = DateTime.Now.AddDays(30);
                                        var transac_type = (from TT in _context.Set<TransactionType>()
                                                            where TT.TransactionCode == "LICENSEFEE"
                                                            select TT).FirstOrDefault();

                                        int id = await UpdateCompanyTranscationType(CompanyID, NewUserId, TimeZoneId, transac_type.TransactionTypeId, transac_type.Rate, 0, "MONTHLY", NextRun);

                                        _context.Set<Registration>().Remove(reg);
                                        _context.SaveChanges();

                                        //Confirm the account and send the account details to the user.
                                        await _SDE.CompanySignUpConfirm(reg.Email, reg.FirstName + " " + reg.LastName, reg.MobileIsd + reg.MobileNo, reg.PaymentMethod, Plan, reg.Password, CompanyID);

                                        UserDTO.companyid = CompanyID;
                                        UserDTO.userid = NewUserId;
                                        UserDTO.Password = reg.Password;
                                        UserDTO.CustomerId = reg.CustomerId;
                                        UserDTO.Token = UniqueId;
                                        UserDTO.ErrorId = 0;
                                        UserDTO.Message = "Validated Sucessfully";

                                        string companyname = string.IsNullOrEmpty(reg.CompanyName) ? "New Company " + CompanyID : reg.CompanyName;
                                        CustomerId = string.IsNullOrEmpty(reg.CustomerId) ? "newcompany" + CompanyID : reg.CustomerId;

                                        try
                                        {
                                           await InsertCompanyApi(CompanyID, companyname, reg.CustomerId, InvitationCode);
                                        }
                                        catch (Exception ex)
                                        {
                                           await DeleteCompanyApi(CompanyID, CustomerId);
                                        }
                                    }
                                    scope.Complete();
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                            else
                            {
                                UserDTO.ErrorId = 100;
                                UserDTO.Message = "User not created";
                            }
                        }
                    }
                }
                return UserDTO;
            }
            catch (Exception ex)
            {

               await DeleteCompanyApi(CompanyID, CustomerId);
                return null;
            }
        }

        public async Task<bool> DeleteTempRegistration(TempRegister tempRegister)
        {
            try
            {
                var reg = await _context.Set<Registration>().Where(t => t.UniqueReference == tempRegister.UniqueRef).FirstOrDefaultAsync();
                if (reg != null)
                {
                    _context.Set<Registration>().Remove(reg);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Sectors>> BusinessSector()
        {
            try
            {
                var result = await _context.Set<Sectors>().FromSqlRaw("EXEC Pro_Get_IndustrySector").ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private  async Task<int> CreateUsers(int companyId, bool registeredUser, string firstName, string primaryEmail, string password,
      int status, int createdUpdatedBy, string timeZoneId, string lastName = "", string mobileNo = "", string userRole = "",
      string userPhoto = "no-photo.jpg", string isdCode = "", string llIsdCode = "", string landLine = "", string secondaryEmail = "",
      string uniqueGuiD = "", string lat = "", string lng = "", string token = "", bool expirePassword = true, string userLanguage = "en",
      bool smsTrigger = false, bool firstLogin = true, int departmentId = 0)
        {
            try
            {

                bool check_Email = await DuplicateEmail(primaryEmail);
                if (check_Email)
                    return 0;

                User NewUsers = new User();

                NewUsers.CompanyId = companyId;
                NewUsers.RegisteredUser = registeredUser;
                NewUsers.FirstName = firstName;

                if (!string.IsNullOrEmpty(lastName))
                    NewUsers.LastName = lastName;

                if (!string.IsNullOrEmpty(isdCode))
                {
                    NewUsers.Isdcode = _DBC.Left(isdCode, 1) != "+" ? "+" + isdCode : isdCode;
                }

                if (!string.IsNullOrEmpty(mobileNo))
                    NewUsers.MobileNo =await _DBC.FixMobileZero(mobileNo);

                if (!string.IsNullOrEmpty(llIsdCode))
                    NewUsers.Llisdcode = _DBC.Left(llIsdCode, 1) != "+" ? "+" + llIsdCode : llIsdCode;

                if (!string.IsNullOrEmpty(landLine))
                    NewUsers.Landline =await _DBC.FixMobileZero(landLine);

                NewUsers.PrimaryEmail = primaryEmail.ToLower();
                NewUsers.UserHash = await _DBC.PWDencrypt(primaryEmail.ToLower());

                if (!string.IsNullOrEmpty(secondaryEmail))
                    NewUsers.SecondaryEmail = secondaryEmail;

                NewUsers.Password = password;

                if (!string.IsNullOrEmpty(uniqueGuiD))
                    NewUsers.UniqueGuiId = uniqueGuiD;
                else
                    NewUsers.UniqueGuiId = Guid.NewGuid().ToString();

                NewUsers.Status = status;

                if (!string.IsNullOrEmpty(userPhoto))
                    NewUsers.UserPhoto = userPhoto;

                if (!string.IsNullOrEmpty(userRole))
                {
                    NewUsers.UserRole = userRole.ToUpper().Replace("STAFF", "USER");
                }
                else
                {
                    NewUsers.UserRole = "USER";
                }

                if (!string.IsNullOrEmpty(lat))
                    NewUsers.Lat = _DBC.Left(lat, 15);

                if (!string.IsNullOrEmpty(lng))
                    NewUsers.Lng = _DBC.Left(lng, 15);

                string CompExpirePwd =await _DBC.GetCompanyParameter("EXPIRE_PASSWORD", companyId);

                if (CompExpirePwd == "true")
                {
                    NewUsers.ExpirePassword = expirePassword;
                }
                else
                {
                    NewUsers.ExpirePassword = false;
                }

                NewUsers.UserLanguage = userLanguage;
                NewUsers.PasswordChangeDate = await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                NewUsers.FirstLogin = firstLogin;

                NewUsers.CreatedBy = createdUpdatedBy;
                NewUsers.CreatedOn =await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                NewUsers.UpdatedBy = createdUpdatedBy;
                NewUsers.UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                NewUsers.TrackingStartTime = SqlDateTime.MinValue.Value;
                NewUsers.TrackingEndTime = SqlDateTime.MinValue.Value;
                NewUsers.LastLocationUpdate = SqlDateTime.MinValue.Value;
                NewUsers.DepartmentId = departmentId;
                NewUsers.Otpexpiry = SqlDateTime.MinValue.Value;

                var roles = await _DBC.CCRoles(true);
                NewUsers.Smstrigger = (roles.Contains(NewUsers.UserRole.ToUpper()) ? smsTrigger : false);

                await _context.Set<User>().AddAsync(NewUsers);
                await _context.SaveChangesAsync();

                if (createdUpdatedBy <= 0)
                {
                    var usr = await _context.Set<User>().Where(t => t.UserId == NewUsers.UserId).FirstOrDefaultAsync();
                    usr.CreatedBy = NewUsers.UserId;
                    usr.UpdatedBy = NewUsers.UserId;
                    await _context.SaveChangesAsync();
                }

                await AddPwdChangeHistory(NewUsers.UserId, password, timeZoneId);

                await CreateUserSearch(NewUsers.UserId, firstName, lastName, isdCode, mobileNo, primaryEmail, companyId);

                await CreateSMSTriggerRight(CompanyId, NewUsers.UserId, NewUsers.UserRole, smsTrigger, NewUsers.Isdcode, NewUsers.MobileNo);

                return NewUsers.UserId;
            }
            catch (Exception ex)
            {
                throw new UserNotFoundException(CompanyId, UserID);
            }
            return 0;
        }
        private async Task<int> AddPwdChangeHistory(int userId, string newPassword, string timeZoneId)
        {

            var ph = new PasswordChangeHistory
            {
                UserId = userId,
                LastPassword = newPassword,
                ChangedDateTime = DateTime.Now.GetDateTimeOffset(timeZoneId)
            };

            _context.Add(ph);
            _context.SaveChanges();
            return ph.Id;

        }
        public async Task CreateSMSTriggerRight(int CompanyId, int UserId, string UserRole, bool SMSTrigger, string ISDCode, string MobileNo, bool Self = false)
        {
            try
            {
                var roles = Roles.CcRoles(true); ;
                var checkusr = await _context.Set<SmsTriggerUser>().FirstOrDefaultAsync(STU => STU.CompanyId == CompanyId && STU.UserId == UserId);
                if (checkusr != null)
                {

                    if (roles.Contains(UserRole.ToUpper()))
                    {
                        checkusr.PhoneNumber = ISDCode + MobileNo;
                    }
                    else
                    {
                        SMSTrigger = false;
                    }

                    if (!SMSTrigger)
                        _context.Remove(checkusr);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (roles.Contains(UserRole.ToUpper()) && SMSTrigger == true)
                    {
                        SmsTriggerUser STU = new SmsTriggerUser();
                        STU.CompanyId = CompanyId;
                        STU.UserId = UserId;
                        STU.PhoneNumber = ISDCode + MobileNo;
                        STU.Status = 1;
                        await _context.AddAsync(STU);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task CreateUserSearch(int userId, string firstName, string lastName, string isdCode, string mobileNo,
                   string primaryEmail, int companyId)
        {
            var searchString = firstName + " " + lastName + "|" + primaryEmail + "|" + isdCode + mobileNo;

            var comp = await _context.Set<Company>().Include(std => std.StdTimeZone).Include(pk => pk.PackagePlan).FirstOrDefaultAsync(x => x.CompanyId == companyId);
            if (comp != null)
            {
                var memberUser = _context.Set<MemberUser>().FromSqlRaw(" exec Pro_Create_User_Search {0}, {1}, {2}",
                    userId, searchString, comp.UniqueKey!).FirstOrDefault();
            }
        }
        private async Task<bool> DuplicateEmail(string email)
        {
            return await _context.Set<User>().Where(t => t.PrimaryEmail == email).AnyAsync();
        }
        private  async Task<int> UpdateCompanyTranscationType(int companyId, int currntUserId, string timeZoneId, int transactionTypeId, decimal transactionRate,
           int compnayTranscationTypeId = 0, string paymentPeriod = "MONTHLY", DateTimeOffset? nextRunDate = null, string paymentMethod = "INVOICE")
        {
            int CTTId = 0;
            if (compnayTranscationTypeId == 0)
            {
                CompanyTranscationType transaction = new CompanyTranscationType();
                if (transactionTypeId > 0)
                    transaction.TransactionTypeID = transactionTypeId;
                transaction.TransactionRate = transactionRate;
                transaction.CompanyId = companyId;
                transaction.PaymentPeriod = paymentPeriod;
                if (nextRunDate.HasValue)
                {
                    transaction.NextRunDate = (DateTimeOffset)nextRunDate;
                }
                else
                {
                    transaction.NextRunDate = await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                }
                transaction.CreatedBy = currntUserId;
                transaction.CreatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                transaction.UpdatedBy = currntUserId;
                transaction.UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                if (!string.IsNullOrEmpty(paymentMethod) && paymentMethod != "UNKNOWN")
                    transaction.PaymentMethod = paymentMethod;

                _context.Set<CompanyTranscationType>().Add(transaction);
                await _context.SaveChangesAsync();
                CTTId = transaction.CompanyTranscationTypeId;
            }
            else
            {
                var newCompanyTranscationType = await (from CTT in _context.Set<CompanyTranscationType>()
                                                       where CTT.CompanyTranscationTypeId == compnayTranscationTypeId && CTT.CompanyId == companyId
                                                       select CTT).FirstOrDefaultAsync();
                if (newCompanyTranscationType != null)
                {
                    if (transactionTypeId > 0)
                        newCompanyTranscationType.TransactionTypeID = transactionTypeId;

                    newCompanyTranscationType.TransactionRate = transactionRate;
                    newCompanyTranscationType.PaymentPeriod = paymentPeriod;
                    newCompanyTranscationType.PaymentMethod = paymentMethod;

                    if (nextRunDate.HasValue)
                    {
                        newCompanyTranscationType.NextRunDate = (DateTimeOffset)nextRunDate;
                    }
                    newCompanyTranscationType.UpdatedBy = currntUserId;
                    newCompanyTranscationType.UpdatedOn = await _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);
                    await _context.SaveChangesAsync();
                    CTTId = newCompanyTranscationType.CompanyTranscationTypeId;
                }
            }
            return CTTId;
        }
    }
    
}
