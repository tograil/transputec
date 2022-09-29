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

    }
}
