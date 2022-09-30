using CrisesControl.Core.Companies;
using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Locations;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class SendEmailService : ISenderEmailService
    {
        private readonly CrisesControlContext _context;
        private readonly IDBCommonRepository _DBC;
        public SendEmailService(CrisesControlContext context, IDBCommonRepository DBC)
        {
            this._context=context;
            this._DBC = DBC;
        }
        public async Task<bool> Email(string[] ToAddress, string MessageBody, string FromAddress, string Provider, string Subject, System.Net.Mail.Attachment fileattached = null)
        {
            try
            {
                bool emailstatus = false;

                if (Provider.ToUpper() == "OFFICE365")
                {
                    string Office365Host = await _DBC.LookupWithKey("AWS_SMTP_HOST");
                    emailstatus = await Office365(ToAddress, MessageBody, FromAddress, Office365Host, Subject, fileattached);
                    //} else if(Provider.ToUpper() == "SENDGRID") {
                    //    emailstatus = SendGridEmail(ToAddress, MessageBody, FromAddress, SendGridAPIKey, Subject);
                }
                else if (Provider.ToUpper() == "AWSSES")
                {
                    emailstatus = await AmazonSESEmail(ToAddress, MessageBody, FromAddress, Subject, fileattached);
                }

                return emailstatus;
            }
            catch (Exception ex)
            {
               throw ex;
                return false;
            }
        }
        public async Task<bool> Office365(string[] ToAddress, string MessageBody, string FromAddress, string Host, string Subject, System.Net.Mail.Attachment fileattached = null)
        {
            
            try
            {

                MailAddress from = new MailAddress(FromAddress);

                MailMessage Message = new MailMessage();
                Message.From = from;

                foreach (string toaddress in ToAddress)
                    Message.To.Add(new MailAddress(toaddress));

                if (fileattached != null)
                {
                    Message.Attachments.Add(fileattached);
                }

                Message.Subject = Subject;
                Message.Body = MessageBody;
                Message.BodyEncoding = Encoding.UTF8;
                Message.IsBodyHtml = true;
                Message.Priority = MailPriority.High;
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Host = Host;
                smtpClient.Send(Message);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }
        public async Task<bool> AmazonSESEmail(string[] ToAddress, string MessageBody, string FromAddress, string Subject, System.Net.Mail.Attachment fileattached = null)
        {
          
            try
            {
                string AWSSESHost = await _DBC.LookupWithKey("AWS_SMTP_HOST");
                string AWSSESUser = await _DBC.LookupWithKey("AWS_SMTP_USER");
                string AWSSESPwd = await _DBC.LookupWithKey("AWS_SMTP_PWD");
                int AWSSESPort = Convert.ToInt32(await _DBC.LookupWithKey("AWS_SMTP_PORT"));
                bool AWSSESSSL = Convert.ToBoolean(await _DBC.LookupWithKey("AWS_SMTP_SSL"));

                MailAddress from = new MailAddress(FromAddress);

                MailMessage Message = new MailMessage();
                Message.From = from;

                foreach (string toaddress in ToAddress)
                    Message.To.Add(new MailAddress(toaddress));

                if (fileattached != null)
                {
                    Message.Attachments.Add(fileattached);
                }

                Message.Subject = Subject;
                Message.Body = MessageBody;
                Message.BodyEncoding = Encoding.UTF8;
                Message.IsBodyHtml = true;
                Message.Priority = MailPriority.High;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = AWSSESHost;
                smtpClient.EnableSsl = AWSSESSSL;
                smtpClient.Port = AWSSESPort;
                smtpClient.Credentials = new NetworkCredential(AWSSESUser, AWSSESPwd);

                smtpClient.Send(Message);
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
                return false;

            }
        }
        private string LookupWithKey(string Key, string Default = "")
        {
            try
            {
                var LKP = _context.Set<SysParameter>()
                          .Where(L=> L.Name == Key).FirstOrDefault();
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
        private async Task<string> UserName(UserFullName strUserName)
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

        public async Task<string> PhoneNumber(PhoneNumber strPhoneNumber)
        {
            try
            {
                if (strPhoneNumber != null)
                {
                    return strPhoneNumber.ISD + strPhoneNumber.Number;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "";
        }
        public async Task KeyContactEmail(string tmpltype, int userId, int inciActId, string statusReason = "")
        {
            try
            {
                var userInfo = await (from U in _context.Set<User>()
                                join C in _context.Set<Company>() on U.CompanyId equals C.CompanyId
                                where U.UserId == userId && U.Status == 1
                                select new
                                {
                                    Username = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName },
                                    Email = U.PrimaryEmail,
                                    CompanyID = C.CompanyId,
                                    CompanyName = C.CompanyName,
                                    CompanyLogo = C.CompanyLogoPath,
                                    C.CustomerId
                                }).FirstOrDefaultAsync();

                if (userInfo != null)
                {
                    string template = string.Empty;

                    if (tmpltype == "initiate")
                    {
                        template = "INITIATE_INCIDENT";
                    }
                    else if (tmpltype == "deactivate")
                    {
                        template = "DEACTIVATE_INCIDENT_CONFIRMED";
                    }
                    else if (tmpltype == "cancel")
                    {
                        template = "CANCEL_INCIDENT";
                    }

                    //string path = Convert.ToString(DBC.LookupWithKey("API_TEMPLATE_PATH")) + filename;
                    string Subject = string.Empty;
                    string message = Convert.ToString(await _DBC.ReadHtmlFile(template, "DB", userInfo.CompanyID,  Subject));


                    string hostname = await _DBC.LookupWithKey("SMTPHOST");
                    string fromadd = await _DBC.LookupWithKey("EMAILFROM");
                    if ((message != null) && (hostname != null) && (fromadd != null))
                    {
                        string messagebody = message;

                        var InciRow = (from IA in _context.Set<IncidentActivation>()
                                       join L in _context.Set<Location>() on IA.ImpactedLocationId equals L.LocationId
                                       //join I in db.Incident on IA.IncidentId equals I.IncidentId
                                       where IA.IncidentActivationId == inciActId
                                       select new
                                       {
                                           IA.IncidentIcon,
                                           IncidentName = IA.Name,
                                           IncidentLocation = L.LocationName,
                                           ActivatedBy = _context.Set<User>().Where(u => u.UserId == IA.InitiatedBy).Select(a => new UserFullName { Firstname = a.FirstName, Lastname = a.LastName }).FirstOrDefault(),
                                           ActivatedOn = IA.InitiatedOn,
                                           LaunchedBy = _context.Set<User>().Where(u => u.UserId == IA.LaunchedBy).Select(a => new UserFullName { Firstname = a.FirstName, Lastname = a.LastName }).FirstOrDefault(),
                                           IA.LaunchedOn,
                                           DeactivatedBy = _context.Set<User>().Where(u => u.UserId == IA.DeactivatedBy).Select(a => new UserFullName { Firstname = a.FirstName, Lastname = a.LastName }).FirstOrDefault(),
                                           IA.DeactivatedOn,
                                           ClosedBy = _context.Set<User>().Where(u => u.UserId == IA.ClosedBy).Select(a => new UserFullName { Firstname = a.FirstName, Lastname = a.LastName }).FirstOrDefault(),
                                           IA.ClosedOn,
                                           UpdatedBy = _context.Set<User>().Where(u => u.UserId == IA.UpdatedBy).Select(a => new UserFullName { Firstname = a.FirstName, Lastname = a.LastName }).FirstOrDefault(),
                                           IA.UpdatedOn,
                                           CurrentStatus = IA.Status,
                                       }).FirstOrDefault();

                        string inidate = string.Empty;
                        string actionby = string.Empty;

                        if (tmpltype == "initiate")
                        {
                            actionby = await _DBC.UserName(InciRow.ActivatedBy);
                            inidate = InciRow.ActivatedOn.ToString();
                        }
                        else if (tmpltype == "deactivate")
                        {
                            actionby = await _DBC.UserName(InciRow.DeactivatedBy);
                            inidate = InciRow.DeactivatedOn.ToString();
                        }
                        else if (tmpltype == "cancel")
                        {
                            actionby = await _DBC.UserName(InciRow.UpdatedBy);
                            inidate = InciRow.UpdatedOn.ToString();
                        }

                        string domain = string.Empty;
                        string LoginLink = string.Empty;
                        string twiterlink = string.Empty;
                        string twitericon = string.Empty;
                        string linkedinlink = string.Empty;
                        string linkedinicon = string.Empty;
                        string facebooklink = string.Empty;
                        string facebookicon = string.Empty;
                        string usersupportlink = string.Empty;


                        twiterlink = await _DBC.LookupWithKey("CC_TWITTER_PAGE");
                        facebooklink = await _DBC.LookupWithKey("CC_FB_PAGE");
                        linkedinlink = await _DBC.LookupWithKey("CC_LINKEDIN_PAGE");
                        domain = await _DBC.LookupWithKey("DOMAIN");
                        twitericon = await _DBC.LookupWithKey("CC_TWITTER_ICON");
                        facebookicon = await _DBC.LookupWithKey("CC_FB_ICON");
                        linkedinicon = await _DBC.LookupWithKey("CC_LINKEDIN_ICON");
                        usersupportlink = await _DBC.LookupWithKey("CC_USER_SUPPORT_LINK");
                        LoginLink = await _DBC.LookupWithKey("PORTAL");


                        string CompanyLogo = LoginLink + "/uploads/" + userInfo.CompanyID + "/companylogos/" + userInfo.CompanyLogo;
                        if (string.IsNullOrEmpty(userInfo.CompanyLogo))
                        {
                            CompanyLogo = await _DBC.LookupWithKey("CCLOGO");
                        }


                        messagebody = messagebody.Replace("{RECIPIENT_NAME}", await _DBC.UserName(userInfo.Username));
                        messagebody = messagebody.Replace("{INCIDENT_ICON}", InciRow.IncidentIcon == null || InciRow.IncidentIcon == "" ? "" : LoginLink + "/assets/images/incident-icons/" + InciRow.IncidentIcon);
                        messagebody = messagebody.Replace("{DEACTIVATED_BY}", actionby);
                        messagebody = messagebody.Replace("{DEACTIVATION_DATE}", inidate);
                        messagebody = messagebody.Replace("{CLOSED_BY}", actionby);
                        messagebody = messagebody.Replace("{CLOSED_DATE}", inidate);
                        messagebody = messagebody.Replace("{LAUNCHED_BY}", actionby);

                        messagebody = messagebody.Replace("{INITIATED_BY}", actionby);
                        messagebody = messagebody.Replace("{INITIATED_DATE}", inidate);
                        messagebody = messagebody.Replace("{REASON}", statusReason);
                        messagebody = messagebody.Replace("{COMPANY_NAME}", userInfo.CompanyName);
                        messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                        messagebody = messagebody.Replace("{INCIDENT_NAME}", InciRow.IncidentName);
                        messagebody = messagebody.Replace("{LOCATION_NAME}", InciRow.IncidentLocation);


                        messagebody = messagebody.Replace("{CC_LOGO}", LoginLink + "/assets/images/logo.png");
                        messagebody = messagebody.Replace("{PORTAL}", LoginLink);

                        messagebody = messagebody.Replace("{TWITTER_LINK}", twiterlink);
                        messagebody = messagebody.Replace("{TWITTER_ICON}", twitericon);
                        messagebody = messagebody.Replace("{FACEBOOK_LINK}", facebooklink);
                        messagebody = messagebody.Replace("{FACEBOOK_ICON}", facebookicon);
                        messagebody = messagebody.Replace("{LINKEDIN_LINK}", linkedinlink);
                        messagebody = messagebody.Replace("{LINKEDIN_ICON}", linkedinicon);
                        messagebody = messagebody.Replace("{CC_WEBSITE}", domain);
                        messagebody = messagebody.Replace("{CC_USER_SUPPORT_LINK}", usersupportlink);
                        messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", userInfo.Email);
                        messagebody = messagebody.Replace("{CUSTOMER_ID}", userInfo.CustomerId);

                        string[] toEmails = { userInfo.Email };
                        bool ismailsend = await Email(toEmails, messagebody, fromadd, hostname, Subject);
                    }
                }
            }
            catch (Exception ex)
            {
                //ToDo throw exception
            }
        }
        public async Task RegistrationCancelled(string CompanyName, int PlanId, DateTimeOffset RegDate, UserFullName pUserName, string pUserEmail, PhoneNumber pUserMobile)
        {
            var sysparms =await  _context.Set<SysParameter>()
                            .Where(SP=> SP.Name == "PORTAL" || SP.Name == "SMTPHOST" || SP.Name == "EMAILFROM")
                            .Select(SP => new { SP.Name, SP.Value }).ToListAsync();

            string hostname = await _DBC.LookupWithKey("SMTPHOST");
            string fromadd = await _DBC.LookupWithKey("EMAILFROM");

            if ((hostname != null) && (fromadd != null))
            {

                System.Text.StringBuilder adminMsg = new System.Text.StringBuilder();
                string Plan = (PlanId == 1 ? "Business" : "EnterPrise");
                adminMsg.AppendLine("<h2>A company has cancelled their subscription from Crises Control</h2>");
                adminMsg.AppendLine("<p>Below are the company information:</p>");
                adminMsg.AppendLine("<strong>Company Name: </strong>" + CompanyName + "</br>");
                adminMsg.AppendLine("<strong>Registration Date: </strong>" + RegDate.ToString() + "</br>");
                adminMsg.AppendLine("<strong>Plan Name: </strong>" + Plan + "</br>");
                adminMsg.AppendLine("<strong>Primary Email: </strong>" + pUserEmail + "</br>");
                adminMsg.AppendLine("<strong>Contact Person: </strong>" + await UserName(pUserName) + "</br>");
                adminMsg.AppendLine("<strong>Mobile: </strong>" + await PhoneNumber(pUserMobile) + "</br>");
                string[] AdminEmail =  LookupWithKey("SENDFEEDBACKTO").Split(',');

               await Email(AdminEmail, adminMsg.ToString(), fromadd, hostname, CompanyName + ": Crises Control: Membership Cancellation Alert");
            }

        }
        public async Task UsageAlert(int CompanyId)
        {
            try
            {

                var company = (from C in _context.Set<Company>()
                               join CP in _context.Set<CompanyPaymentProfile>() on C.CompanyId equals CP.CompanyId
                               where C.CompanyId == CompanyId
                               select new { C, CP }).FirstOrDefault();
                if (company != null)
                {
                    string bill_status = company.C.CompanyProfile;
                    string templatename = string.Empty;
                    if (bill_status == "STOP_MESSAGING")
                    {
                        templatename = "STOP_MESSAGE_ALERT";
                    }
                    else if (bill_status == "LOW_BALANCE" || bill_status == "ON_CREDIT")
                    {
                        templatename = "LOW_BALANCE_ALERT";
                    }

                    string Subject = string.Empty;
                    string message = Convert.ToString(await _DBC.ReadHtmlFile(templatename, "DB", company.C.CompanyId,  Subject));

                    string Website =await _DBC.LookupWithKey("DOMAIN");
                    string Portal = await _DBC.LookupWithKey("PORTAL");
                    string hostname = await _DBC.LookupWithKey("SMTPHOST");
                    string fromadd = await _DBC.LookupWithKey("ALERT_EMAILFROM");

                    string CompanyLogo = Portal + "/uploads/" + company.C.CompanyId + "/companylogos/" + company.C.CompanyLogoPath;

                    if (string.IsNullOrEmpty(company.C.CompanyLogoPath))
                    {
                        CompanyLogo = await _DBC.LookupWithKey("CCLOGO");
                    }

                    if ((message != null) && (hostname != null) && (fromadd != null))
                    {
                        string messagebody = message;

                        string billing_email = await _DBC.LookupWithKey("BILLING_EMAIL");

                        //Get company billing email list.
                        string billing_users = await _DBC.GetCompanyParameter("BILLING_USERS", company.C.CompanyId);

                        List<string> emaillist = new List<string>();

                        if (!string.IsNullOrEmpty(billing_users))
                        {
                            var user_ids = billing_users.Split(',').Select(Int32.Parse).ToList();
                            if (user_ids.Count > 0)
                            {
                                var get_user = await (from U in _context.Set<User>()
                                                where user_ids.Contains(U.UserId) && U.Status != 3
                                                select new
                                                {
                                                    U.PrimaryEmail
                                                }).ToListAsync();
                                foreach (var bill_user in get_user)
                                {
                                    emaillist.Add(bill_user.PrimaryEmail);
                                }
                            }
                        }

                        messagebody = messagebody.Replace("{COMPANY_NAME}", company.C.CompanyName);
                        messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                        messagebody = messagebody.Replace("{CC_WEBSITE}", Website);
                        messagebody = messagebody.Replace("{PORTAL}", Portal);

                        messagebody = messagebody.Replace("{BILLING_EMAIL}", billing_email);
                        messagebody = messagebody.Replace("{CREDIT_BALANCE}",await _DBC.ToCurrency(company.CP.CreditBalance));
                        messagebody = messagebody.Replace("{MINIMUM_BALANCE}",await _DBC.ToCurrency(company.CP.MinimumBalance));
                        messagebody = messagebody.Replace("{CREDIT_LIMIT}",await _DBC.ToCurrency(company.CP.CreditLimit));

                        List<string> allowed_comms = new List<string>();
                        List<string> stopped_comms = new List<string>();

                        var subscribed_method = (from CM in _context.Set<CompanyComm>()
                                                 join CO in _context.Set<CommsMethod>() on CM.MethodId equals CO.CommsMethodId
                                                 where CM.CompanyId == CompanyId
                                                 select new { CO, CM }).Select(s => new {
                                                     MethodCode = s.CO.MethodCode,
                                                     MethodName = s.CO.MethodName,
                                                     ServiceStats = s.CM.ServiceStatus
                                                 }).ToList();
                        foreach (var method in subscribed_method)
                        {
                            if (method.MethodCode == "EMAIL")
                            {
                                if (company.CP.MinimumEmailRate > 0 && method.ServiceStats == false)
                                {
                                    stopped_comms.Add(method.MethodName);
                                }
                                else
                                {
                                    allowed_comms.Add(method.MethodName);
                                }
                            }
                            if (method.MethodCode == "PUSH")
                            {
                                if (company.CP.MinimumPushRate > 0 && method.ServiceStats == false)
                                {
                                    stopped_comms.Add(method.MethodName);
                                }
                                else
                                {
                                    allowed_comms.Add(method.MethodName);
                                }
                            }
                            if (method.MethodCode == "TEXT")
                            {
                                if (company.CP.MinimumTextRate > 0 && method.ServiceStats == false)
                                {
                                    stopped_comms.Add(method.MethodName);
                                }
                                else
                                {
                                    allowed_comms.Add(method.MethodName);
                                }
                            }
                            if (method.MethodCode == "PHONE")
                            {
                                if (company.CP.MinimumPhoneRate > 0 && method.ServiceStats == false)
                                {
                                    stopped_comms.Add(method.MethodName);
                                }
                                else
                                {
                                    allowed_comms.Add(method.MethodName);
                                }
                            }
                        }

                        messagebody = messagebody.Replace("{STOPPED_COMMS}", string.Join(",", stopped_comms));
                        messagebody = messagebody.Replace("{ALLOWED_COMMS}", string.Join(",", allowed_comms));
                        messagebody = messagebody.Replace("{CUSTOMER_ID}", company.C.CustomerId);

                        Subject = Subject + " " + company.C.CompanyName;
                        string[] toEmails = emaillist.ToArray();

                        string[] adm_email = { billing_email };

                        await Email(adm_email, messagebody, fromadd, hostname, Subject);

                        string cust_usage_alert = await _DBC.LookupWithKey("SEND_USAGE_ALERT_TO_CUSTOMER");
                        if (cust_usage_alert == "true")
                          await  Email(toEmails, messagebody, fromadd, hostname, Subject);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task NotifyKeyHolders(string tmpltype, int inciActId, int currentUserid, int companyId, string statusReason = "", bool hasNominatedKH = false)
        {
            try
            {

                var roles = await _DBC.CCRoles(true);
                var keyHolderList = _context.Set<User>().Where(u => (roles.Contains(u.UserRole)) && u.CompanyId == companyId && u.Status == 1).ToList();

                if (hasNominatedKH)
                    keyHolderList = (from U in _context.Set<User>()
                                     join IKH in _context.Set<IncidentKeyholder>() on U.UserId equals IKH.UserID
                                     where (roles.Contains(U.UserRole)) && U.CompanyId == companyId && U.Status == 1
                                     && IKH.ActiveIncidentID == inciActId && U.ActiveOffDuty == 0
                                     select U).ToList();

                List<int> userToNotify = new List<int>();

                foreach (var user in keyHolderList)
                {
                   await KeyContactEmail(tmpltype, user.UserId, inciActId, statusReason);
                    userToNotify.Add(user.UserId);
                }

                bool sendPing = Convert.ToBoolean(_DBC.GetCompanyParameter("NOTIFY_KEYHOLDER_BY_PING", companyId));

                //Making the http call to send a ping to all key holders.

                if (sendPing)
                {
                    string APIBaseURL = await _DBC.LookupWithKey("APIBASEURL");

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(APIBaseURL + _DBC.getapiversion());

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string messageText = string.Empty;

                    string portalUrl = await _DBC.LookupWithKey("PORTAL");

                    if (tmpltype == "initiate")
                    {
                        messageText = "A new Incident has been initiated. Please login to the portal (" + portalUrl + ") and perform the required action";
                        inciActId = 0;
                    }
                    else if (tmpltype == "deactivate")
                    {
                        messageText = "An Incident has been deactivated. Please login to the portal (" + portalUrl + ") and perform the required action";
                    }
                    else if (tmpltype == "cancel")
                    {
                        messageText = "An Incident has been cancelled. Please login to the portal (" + portalUrl + ") and perform the required action";
                    }

                    string userpwd =await _context.Set<User>().Where(w => w.UserId == currentUserid).Select(s => s.Password).FirstOrDefaultAsync();
                    userpwd =await _DBC.PWDencrypt(userpwd);

                    string commsMethod = await _DBC.GetCompanyParameter("DEFAULT_AWAITING_INCIDENT_CHANNEL", companyId);
                    int[] messageMethod = null;
                    if (!string.IsNullOrEmpty(commsMethod))
                    {
                        messageMethod = commsMethod.Split(',').Select(int.Parse).ToArray();
                    }

                    var newPingMessageModel = new PingMessageModel()
                    {
                        CompanyId = companyId,
                        CurrentUserId = currentUserid,
                        Password = userpwd,
                        IncidentActivationId = inciActId,
                        Priority = 999,
                        MessageText = messageText,
                        MessageType = "Ping",
                        MultiResponse = false,
                        AckOptions = null,
                        PingMessageObjLst = null,
                        UsersToNotify = userToNotify.ToArray(),
                        AudioAssetId = 0,
                        MessageMethod = messageMethod
                    };

                    var responsePing = client.PostAsJsonAsync("Messaging/PingMessage", newPingMessageModel).Result;
                    Task<string> ResultStringPing = responsePing.Content.ReadAsStringAsync();
                    if (responsePing.IsSuccessStatusCode)
                    {
                        string ressultstrIncidentPing = ResultStringPing.Result.Trim();
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                //ToDo: Throw exception
            }
        }
        public async Task SendUserAssociationsToAdmin(string items, int userId, int companyId)
        {
            try
            {
                var roles = await _DBC.CCRoles();
                var user = _context.Set<User>().Where(t => t.UserId == userId).Select(t => new UserFullName { Firstname = t.FirstName, Lastname = t.LastName }).FirstOrDefault();

                var adminuser = await  _context.Set<User>()
                                 .Where(U=> roles.Contains(U.UserRole) && U.CompanyId == companyId && U.Status == 1)
                                 .Select(U=> new { U.PrimaryEmail, U.FirstName, U.LastName }).ToListAsync();

                var company = await _context.Set<Company>().Include(CP=>CP.CompanyPaymentProfiles)
                              // join CP in _context.Set<CompanyPaymentProfile>() on C.CompanyId equals CP.CompanyId
                               .Where(C=> C.CompanyId == companyId)
                               .FirstOrDefaultAsync();
                if (company != null)
                {
                    string templatename = "USER_DELETE_ALERT";
                    string Subject = string.Empty;
                    string message = Convert.ToString(await _DBC.ReadHtmlFile(templatename, "DB", company.CompanyId,  Subject));

                    string Website = await  _DBC.LookupWithKey("DOMAIN");
                    string Portal = await _DBC.LookupWithKey("PORTAL");

                    string hostname = await _DBC.LookupWithKey("SMTPHOST");
                    string fromadd = await _DBC.LookupWithKey("EMAILFROM");
                    string CompanyLogo = Portal + "/uploads/" + company.CompanyId + "/companylogos/" + company.CompanyLogoPath;

                    if (string.IsNullOrEmpty(company.CompanyLogoPath))
                    {
                        CompanyLogo = await _DBC.LookupWithKey("CCLOGO");
                    }

                    if ((message != null) && (hostname != null) && (fromadd != null))
                    {
                        string messagebody = message;

                        messagebody = messagebody.Replace("{COMPANY_NAME}", company.CompanyName);
                        messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                        messagebody = messagebody.Replace("{CC_WEBSITE}", Website);
                        messagebody = messagebody.Replace("{PORTAL}", Portal);
                        messagebody = messagebody.Replace("{DELETED_USER_NAME}", user.Firstname + " " + user.Lastname);
                        messagebody = messagebody.Replace("{USER_LINKS}", items);
                        messagebody = messagebody.Replace("{CUSTOMER_ID}", company.CustomerId);

                        foreach (var admin in adminuser)
                        {
                            string sendbody = messagebody;
                            sendbody = sendbody.Replace("{RECIPIENT_NAME}", admin.FirstName + " " + admin.LastName);
                            string[] adm_email = { admin.PrimaryEmail };
                           await Email(adm_email, sendbody, fromadd, hostname, Subject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //ToDo: throw exception
            }
        }
        public async Task NewUserAccount(string emailId, string userName, int companyId, string guid)
        {
            try
            {
                string Subject = string.Empty;
                string message = Convert.ToString(await _DBC.ReadHtmlFile("NEW_USER_ACCOUNT", "DB", companyId, Subject))!;
                var company = _context.Set<Company>().Where(c => c.CompanyId == companyId).FirstOrDefault();

                string website =await _DBC.LookupWithKey("DOMAIN");
                string portal =await _DBC.LookupWithKey("PORTAL");
                string valdiateURL =await _DBC.LookupWithKey("EMAIL_VALIDATE_URL");
                string accountDeleteURL =await _DBC.LookupWithKey("EMAIL_VALIDATE_ACCOUNT_DELETE");

                string verifylink = portal + valdiateURL + companyId + "/" + guid;
                string deleteVerifyLink = portal + accountDeleteURL + companyId + "/" + guid;

                string hostname =await _DBC.LookupWithKey("SMTPHOST");
                string fromadd =await _DBC.LookupWithKey("EMAILFROM");
                string companyLogo = portal + "/uploads/" + companyId + "/companylogos/" + company?.CompanyLogoPath;

                if (string.IsNullOrEmpty(company?.CompanyLogoPath))
                {
                    companyLogo =await _DBC.LookupWithKey("CCLOGO");
                }


                if ((message != null) && (hostname != null) && (fromadd != null))
                {

                    string messagebody = message;

                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", userName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", emailId);
                    messagebody = messagebody.Replace("{COMPANY_NAME}", company?.CompanyName);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", companyLogo);
                    messagebody = messagebody.Replace("{PORTAL}", portal);
                    messagebody = messagebody.Replace("{VERIFY_LINK}", verifylink);
                    messagebody = messagebody.Replace("{DELETE_ACCOUNT_LINK}", deleteVerifyLink);
                    messagebody = messagebody.Replace("{CC_WEBSITE}", website);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", company?.CustomerId);

                    messagebody = messagebody.Replace("{TWITTER_LINK}",await _DBC.LookupWithKey("CC_TWITTER_PAGE"));
                    messagebody = messagebody.Replace("{TWITTER_ICON}",await _DBC.LookupWithKey("CC_TWITTER_ICON"));
                    messagebody = messagebody.Replace("{FACEBOOK_LINK}",await _DBC.LookupWithKey("CC_FB_PAGE"));
                    messagebody = messagebody.Replace("{FACEBOOK_ICON}",await _DBC.LookupWithKey("CC_FB_ICON"));
                    messagebody = messagebody.Replace("{LINKEDIN_LINK}",await _DBC.LookupWithKey("CC_LINKEDIN_PAGE"));
                    messagebody = messagebody.Replace("{LINKEDIN_ICON}",await _DBC.LookupWithKey("CC_LINKEDIN_ICON"));

                    string[] toEmails = { emailId };
                    bool ismailsend =await Email(toEmails, messagebody, fromadd, hostname, Subject);
                }
            }
            catch (Exception ex)
            {
                //ToDo: throw exception
            }
        }

        public async Task SendReviewAlert(int incidentId, int headerId, int companyId, string reminderType = "TASK")
        {
            try
            {

                string path = "TASK_REVIEW_REMINDER";

                if (reminderType == "SOP")
                {
                    path = "SOP_REVIEW_REMINDER";
                }
                string subject = string.Empty;
                string message = Convert.ToString(await _DBC.ReadHtmlFile(path, "DB", companyId,  subject))!;


                var companyInfo = _context.Set<Company>().Where(c => c.CompanyId == companyId).FirstOrDefault()!;
                string website =await _DBC.LookupWithKey("DOMAIN");
                string portal =await _DBC.LookupWithKey("PORTAL");
                string hostname =await _DBC.LookupWithKey("SMTPHOST");
                string fromadd =await _DBC.LookupWithKey("EMAILFROM");
                string companyLogo = portal + "/uploads/" + companyInfo.CompanyId + "/companylogos/" + companyInfo.CompanyLogoPath;
                if (string.IsNullOrEmpty(companyInfo.CompanyLogoPath))
                {
                    companyLogo =await _DBC.LookupWithKey("CCLOGO");
                }
                string incidentname = string.Empty;
                string incidentimage = portal + "/assets/images/incident-icons/";

                DateTimeOffset review_date = DateTimeOffset.Now;
                int sendtoid = 0;

                string emailmessage = string.Empty;
                if (reminderType.ToUpper() == "TASK")
                {
                    var incidentrow = (from I in _context.Set<Incident>()
                                       join TH in _context.Set<TaskHeader>() on I.IncidentId equals TH.IncidentId
                                       where I.IncidentId == incidentId
                                       select new { I, TH }).FirstOrDefault();

                    if (incidentrow != null)
                    {
                        sendtoid = (int)incidentrow.TH.Author;
                        incidentname = incidentrow.I.Name;
                        incidentimage = incidentrow.I.IncidentIcon;
                        review_date = incidentrow.TH.NextReviewDate;
                        subject = subject + "Incident task review reminder";
                        emailmessage = "This is the reminder for you to review the tasks for the following incident.";
                        if (incidentrow.TH.ReminderCount == 3)
                        {
                            emailmessage = "<span style='color:#ff0000'>This is the final reminder for you to review the tasks for the following incident.</span>";
                        }
                    }
                }
                else if (reminderType.ToUpper() == "SOP")
                {
                    var incidentrow = (from I in _context.Set<Incident>()
                                       join TH in _context.Set<IncidentSop>() on I.IncidentId equals TH.IncidentId
                                       join SH in _context.Set<Sopheader>() on TH.SopheaderId equals SH.SopheaderId
                                       where I.IncidentId == incidentId && SH.SopheaderId == headerId
                                       select new { I, SH }).FirstOrDefault();

                    if (incidentrow != null)
                    {
                        incidentname = incidentrow.I.Name!;
                        incidentimage = incidentrow.I.IncidentIcon!;
                        review_date = incidentrow.SH.ReviewDate;
                        emailmessage = "This is the reminder for you to review the SOP document for the following incident.";
                        if (incidentrow.SH.ReminderCount == 3)
                        {
                            emailmessage = "<span style='color:#ff0000'>This is the final reminder for you to review the SOP document for the following incident</span>";
                        }
                    }
                }


                if ((message != null) && (hostname != null) && (fromadd != null))
                {
                    string messagebody = message;

                    messagebody = messagebody.Replace("{COMPANY_NAME}", companyInfo.CompanyName);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", companyLogo);
                    messagebody = messagebody.Replace("{CC_WEBSITE}", website);
                    messagebody = messagebody.Replace("{PORTAL}", portal);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", companyInfo.CustomerId);

                    messagebody = messagebody.Replace("{INCIDENT_NAME}", incidentname);
                    messagebody = messagebody.Replace("{INCIDENT_ICON}", portal + "assets/images/incident-icons/" + incidentimage);
                    messagebody = messagebody.Replace("{SOP_REVIEW_DATE}", review_date.ToString("dd-MMM-yy"));
                    messagebody = messagebody.Replace("{INCIDENT_REVIEW_DATE}", review_date.ToString("dd-MMM-yy"));
                    messagebody = messagebody.Replace("{INCIDENT_MESSAGE}", emailmessage);

                    if (reminderType == "TASK" && sendtoid > 0)
                    {
                        var kc = await  _context.Set<User>()
                                  .Where(U=> U.UserId == sendtoid && U.Status == 1)
                                  .Select(U=> new
                                  {
                                      UserName = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName },
                                      U.PrimaryEmail
                                  }).ToListAsync();

                        foreach (var k in kc)
                        {
                            string sendbody = messagebody;
                            sendbody = sendbody.Replace("{RECIPIENT_NAME}", await _DBC.UserName(k.UserName));
                            sendbody = sendbody.Replace("{RECIPIENT_EMAIL}", k.PrimaryEmail);
                            string[] toEmails = { k.PrimaryEmail };
                            bool ismailsend = await Email(toEmails, sendbody, fromadd, hostname, subject);
                        }
                    }
                    else if (reminderType == "SOP")
                    {
                        var owners = (from OW in _context.Set<SopdetailGroup>()
                                      join SD in _context.Set<Sopdetail>() on OW.SopdetailId equals SD.SopdetailId
                                      join SC in _context.Set<ContentSection>() on SD.ContentSectionId equals SC.ContentSectionId
                                      join U in _context.Set<User>() on OW.SopgroupId equals U.UserId
                                      where SD.SopheaderId == headerId && U.CompanyId == companyId
                                      orderby U.UserId
                                      select new { U.UserId, UserName = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName }, U.PrimaryEmail, SC.SectionName }).ToList();

                        int CurrentUserID = 0;
                        string CurrentEmail = "";
                        string CurrentUser = "";
                        StringBuilder sections = new StringBuilder();

                        foreach (var k in owners)
                        {

                            if (CurrentUserID != k.UserId && CurrentUserID != 0)
                            {
                                string sendbody = messagebody;
                                sendbody = sendbody.Replace("{RECIPIENT_NAME}", CurrentUser);
                                sendbody = sendbody.Replace("{RECIPIENT_EMAIL}", CurrentEmail);
                                sendbody = sendbody.Replace("{SECTION_CONTENT}", "<ul>" + sections.ToString() + "</ul>");
                                string[] toEmails = { k.PrimaryEmail };
                                bool ismailsend = await Email(toEmails, sendbody, fromadd, hostname, subject);
                            }

                            CurrentUser = await _DBC.UserName(k.UserName);
                            CurrentEmail = k.PrimaryEmail;
                            CurrentUserID = k.UserId;

                            sections.AppendLine("<li>" + k.SectionName + "</li>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //ToDo:
            }
        }
        public async Task NewUserAccountConfirm(string emailId, string userName, string userPass, int companyId, string guid)
        {
            try
            {

                string subject = string.Empty;
                string message = Convert.ToString(await _DBC.ReadHtmlFile("NEW_ACCOUNT_CONFIRMED", "DB", companyId,  subject))!;

                var companyInfo = _context.Set<Company>().Where(c => c.CompanyId == companyId).FirstOrDefault();
                string website = await _DBC.LookupWithKey("DOMAIN");
                string portal =await _DBC.LookupWithKey("PORTAL");
                string valdiateURL = await _DBC.LookupWithKey("EMAIL_VALIDATE_URL");
                string hostname = await _DBC.LookupWithKey("SMTPHOST");
                string fromadd =await _DBC.LookupWithKey("EMAILFROM");
                string sso_login = await _DBC.GetCompanyParameter("AAD_SSO_TENANT_ID", companyId);

                string verifylink = portal + valdiateURL + companyId + "/" + guid;
                string companyLogo = portal + "/uploads/" + companyInfo.CompanyId + "/companylogos/" + companyInfo.CompanyLogoPath;

                if (string.IsNullOrEmpty(companyInfo.CompanyLogoPath))
                {
                    companyLogo = await _DBC.LookupWithKey("CCLOGO");
                }

                if (!string.IsNullOrEmpty(sso_login))
                    userPass = "Use the single sign-on to login";

                if ((message != null) && (hostname != null) && (fromadd != null))
                {
                    string messagebody = message;

                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", userName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", emailId);
                    messagebody = messagebody.Replace("{RECIPIENT_PASSWORD}", userPass);
                    messagebody = messagebody.Replace("{COMPANY_NAME}", companyInfo.CompanyName);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", companyLogo);
                    messagebody = messagebody.Replace("{VERIFY_LINK}", verifylink);
                    messagebody = messagebody.Replace("{CC_WEBSITE}", website);
                    messagebody = messagebody.Replace("{PORTAL}", portal);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", companyInfo.CustomerId);

                    messagebody = messagebody.Replace("{TWITTER_LINK}",await _DBC.LookupWithKey("CC_TWITTER_PAGE"));
                    messagebody = messagebody.Replace("{TWITTER_ICON}",await _DBC.LookupWithKey("CC_TWITTER_ICON"));
                    messagebody = messagebody.Replace("{FACEBOOK_LINK}",await _DBC.LookupWithKey("CC_FB_PAGE"));
                    messagebody = messagebody.Replace("{FACEBOOK_ICON}",await _DBC.LookupWithKey("CC_FB_ICON"));
                    messagebody = messagebody.Replace("{LINKEDIN_LINK}",await _DBC.LookupWithKey("CC_LINKEDIN_PAGE"));
                    messagebody = messagebody.Replace("{LINKEDIN_ICON}",await _DBC.LookupWithKey("CC_LINKEDIN_ICON"));

                    string[] toEmails = { emailId };

                    bool ismailsend = await Email(toEmails, messagebody, fromadd, hostname, subject);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task CompanySignUpConfirm(string emailId, string userName, string mobile, string paymentMethod, string plan, string userPass, int companyId)
        {
            try
            {
                string subject = string.Empty;
                string message = Convert.ToString(await _DBC.ReadHtmlFile("COMPANY_CONFIRMED", "DB", companyId,  subject))!;

            
                var company = (from C in _context.Set<Company>()
                               join CP in _context.Set<CompanyPaymentProfile>() on C.CompanyId equals CP.CompanyId
                               where C.CompanyId == companyId
                               select new { C, CP }).FirstOrDefault();

                string website = await _DBC.LookupWithKey("DOMAIN");
                string portal =await _DBC.LookupWithKey("PORTAL");
                string adminPortal =await _DBC.LookupWithKey("ADMIN_SITE_URL");

                string hostname =await _DBC.LookupWithKey("SMTPHOST");
                string fromadd =await _DBC.LookupWithKey("EMAILFROM");
                string CCimage =await _DBC.LookupWithKey("CCLOGO");


                StringBuilder adminMsg = new StringBuilder();

                adminMsg.AppendLine("<h2>A new company is registered on Crises Control</h2>");
                adminMsg.AppendLine("<p>Below are the company information:</p>");
                adminMsg.AppendLine("<strong>Company Name: </strong>" + company?.C.CompanyName + "(" + company.C.CustomerId + ")</br>");
                adminMsg.AppendLine("<strong>Plan Name: </strong>" + plan + "</br>");
                adminMsg.AppendLine("<strong>Primary Email: </strong>" + emailId + "</br>");
                adminMsg.AppendLine("<strong>Contact Person: </strong>" + userName + "</br>");
                adminMsg.AppendLine("<strong>Mobile: </strong>" + mobile + "</br>");

                //if(Company.PackagePlanId == 2)
                //    adminMsg.AppendLine("<p style=\"color:#ff0000;font-size:18px\"><strong>Please configure the enterprise parameters for the company, <a href=\"" + AdminPortal + "\">configure now</a></p>");

                string[] AdminEmail = await _DBC.LookupWithKey("SENDFEEDBACKTO").Split(',');

                await Email(AdminEmail, adminMsg.ToString(), fromadd, hostname, company.C.CompanyName + ": A new Company registered on crises control");

                if ((message != null) && (hostname != null) && (fromadd != null))
                {
                    string messagebody = message;

                    int TrialDays = (int)DateTimeOffset.Now.Date.Subtract(company.CP.ContractStartDate.Date).TotalDays;

                    messagebody = messagebody.Replace("{TRIALS_END_DAYS}", TrialDays.ToString());
                    messagebody = messagebody.Replace("{TRIALS_END_ON}", company.CP.ContractStartDate.ToString("dd-MM-yy"));
                    messagebody = messagebody.Replace("{TRIAL_END_DATE}", company.CP.ContractStartDate.ToString("dd-MM-yy"));

                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", userName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", emailId);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", company.C.CustomerId);
                    messagebody = messagebody.Replace("{PORTAL}", portal);
                    messagebody = messagebody.Replace("{RECIPIENT_PASSWORD}", userPass);
                    messagebody = messagebody.Replace("{COMPANY_NAME}", company.C.CompanyName);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", CCimage);
                    messagebody = messagebody.Replace("{DOMAIN}", website);

                    string[] toEmails = { emailId };
                    bool ismailsend =await Email(toEmails, messagebody, fromadd, hostname, subject);
                }
            }
            catch (Exception ex)
            {
                //ToDo: throw exception
            }
        }

    }
}
