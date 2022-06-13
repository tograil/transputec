using CrisesControl.Core.Companies;
using CrisesControl.Core.Compatibility.Jobs;
using CrisesControl.Core.Incidents;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Api.Application.Helpers
{
    public class SendEmail
    {

        private readonly CrisesControlContext _context;
        private readonly DBCommon _DBC;

        public SendEmail(CrisesControlContext context, DBCommon DBC)
        {
            _context = context;
            _DBC = DBC;
        }


        public void CompanySignUpConfirm(string emailId, string userName, string mobile, string paymentMethod, string plan, string userPass, int companyId)
        {
            try
            {
                string subject = string.Empty;
                string message = Convert.ToString(_DBC.ReadHtmlFile("COMPANY_CONFIRMED", "DB", companyId, out subject))!;

                var sysparms = (from SP in _context.Set<SysParameter>()
                                where SP.Name == "DOMAIN" || SP.Name == "CC_USER_SUPPORT_LINK" || SP.Name == "ADMIN_SITE_URL"
                                || SP.Name == "PORTAL" || SP.Name == "SMTPHOST" || SP.Name == "EMAILFROM" || SP.Name == "CCLOGO"
                                select new { SP.Name, SP.Value }).ToList();

                var company = (from C in _context.Set<Company>()
                               join CP in _context.Set<CompanyPaymentProfile>() on C.CompanyId equals CP.CompanyId
                               where C.CompanyId == companyId
                               select new { C, CP }).FirstOrDefault();

                string website = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault()!;
                string portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault()!;
                string adminPortal = sysparms.Where(w => w.Name == "ADMIN_SITE_URL").Select(s => s.Value).FirstOrDefault()!;

                string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault()!;
                string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault()!;
                string CCimage = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault()!;


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

                string[] AdminEmail = _DBC.LookupWithKey("SENDFEEDBACKTO").Split(',');

                Email(AdminEmail, adminMsg.ToString(), fromadd, hostname, company.C.CompanyName + ": A new Company registered on crises control");

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
                    bool ismailsend = Email(toEmails, messagebody, fromadd, hostname, subject);
                }
            }
            catch (Exception ex)
            {
                //ToDo: throw exception
            }
        }

        public void NewUserAccount(string emailId, string userName, int companyId, string guid)
        {
            try
            {
                string Subject = string.Empty;
                string message = Convert.ToString(_DBC.ReadHtmlFile("NEW_USER_ACCOUNT", "DB", companyId, out Subject))!;

                var sysparms = (from SP in _context.Set<SysParameter>()
                                where SP.Name == "CC_TWITTER_PAGE" || SP.Name == "CC_FB_PAGE"
                                || SP.Name == "CC_LINKEDIN_PAGE" || SP.Name == "DOMAIN"
                                || SP.Name == "CC_TWITTER_ICON" || SP.Name == "CC_FB_ICON"
                                || SP.Name == "CC_LINKEDIN_ICON" || SP.Name == "CC_USER_SUPPORT_LINK"
                                || SP.Name == "PORTAL" || SP.Name == "SMTPHOST" || SP.Name == "EMAIL_VALIDATE_URL" || SP.Name == "EMAIL_VALIDATE_ACCOUNT_DELETE"
                                || SP.Name == "EMAILFROM" || SP.Name == "CCLOGO"
                                select new { SP.Name, SP.Value }).ToList();


                var company = _context.Set<Company>().Where(c=> c.CompanyId == companyId).FirstOrDefault();

                string website = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault()!;
                string portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault()!;
                string valdiateURL = sysparms.Where(w => w.Name == "EMAIL_VALIDATE_URL").Select(s => s.Value).FirstOrDefault()!;
                string accountDeleteURL = sysparms.Where(w => w.Name == "EMAIL_VALIDATE_ACCOUNT_DELETE").Select(s => s.Value).FirstOrDefault()!;

                string verifylink = portal + valdiateURL + companyId + "/" + guid;
                string deleteVerifyLink = portal + accountDeleteURL + companyId + "/" + guid;

                string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault()!;
                string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault()!;
                string companyLogo = portal + "/uploads/" + companyId + "/companylogos/" + company?.CompanyLogoPath;

                if (string.IsNullOrEmpty(company?.CompanyLogoPath))
                {
                    companyLogo = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault()!;
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

                    messagebody = messagebody.Replace("{TWITTER_LINK}", sysparms.Where(w => w.Name == "CC_TWITTER_PAGE").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{TWITTER_ICON}", sysparms.Where(w => w.Name == "CC_TWITTER_ICON").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{FACEBOOK_LINK}", sysparms.Where(w => w.Name == "CC_FB_PAGE").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{FACEBOOK_ICON}", sysparms.Where(w => w.Name == "CC_FB_ICON").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{LINKEDIN_LINK}", sysparms.Where(w => w.Name == "CC_LINKEDIN_PAGE").Select(s => s.Value).FirstOrDefault());
                    messagebody = messagebody.Replace("{LINKEDIN_ICON}", sysparms.Where(w => w.Name == "CC_LINKEDIN_ICON").Select(s => s.Value).FirstOrDefault());

                    string[] toEmails = { emailId };
                    bool ismailsend = Email(toEmails, messagebody, fromadd, hostname, Subject);
                }
            }
            catch (Exception ex)
            {
                //ToDo: throw exception
            }
        }

        public void SendReviewAlert(int incidentId, int headerId, int companyId, string reminderType = "TASK")
        {
            try
            {

                string path = "TASK_REVIEW_REMINDER";

                if (reminderType == "SOP")
                {
                    path = "SOP_REVIEW_REMINDER";
                }
                string subject = string.Empty;
                string message = Convert.ToString(_DBC.ReadHtmlFile(path, "DB", companyId, out subject))!;

                var sysparms = (from SP in _context.Set<SysParameter>()
                                where SP.Name == "DOMAIN" || SP.Name == "CC_USER_SUPPORT_LINK"
                                || SP.Name == "PORTAL" || SP.Name == "SMTPHOST" || SP.Name == "EMAILFROM" || SP.Name == "CCLOGO"
                                select new { SP.Name, SP.Value }).ToList();


                var companyInfo = _context.Set<Company>().Where(c => c.CompanyId == companyId).FirstOrDefault()!;
                string website = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault()!;
                string portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault()!;
                string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault()!;
                string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault()!;
                string companyLogo = portal + "/uploads/" + companyInfo.CompanyId + "/companylogos/" + companyInfo.CompanyLogoPath;
                if (string.IsNullOrEmpty(companyInfo.CompanyLogoPath))
                {
                    companyLogo = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault()!;
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
                        var kc = (from U in _context.Set<User>()
                                  where U.UserId == sendtoid && U.Status == 1
                                  select new
                                  {
                                      UserName = new UserFullName { Firstname = U.FirstName, Lastname = U.LastName },
                                      U.PrimaryEmail
                                  }).ToList();

                        foreach (var k in kc)
                        {
                            string sendbody = messagebody;
                            sendbody = sendbody.Replace("{RECIPIENT_NAME}", _DBC.UserName(k.UserName));
                            sendbody = sendbody.Replace("{RECIPIENT_EMAIL}", k.PrimaryEmail);
                            string[] toEmails = { k.PrimaryEmail };
                            bool ismailsend = Email(toEmails, sendbody, fromadd, hostname, subject);
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
                                bool ismailsend = Email(toEmails, sendbody, fromadd, hostname, subject);
                            }

                            CurrentUser = _DBC.UserName(k.UserName);
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

        public void NewUserAccountConfirm(string emailId, string userName, string userPass, int companyId, string guid)
        {
            try
            {

                string subject = string.Empty;
                string message = Convert.ToString(_DBC.ReadHtmlFile("NEW_ACCOUNT_CONFIRMED", "DB", companyId, out subject))!;

                var companyInfo = _context.Set<Company>().Where(c => c.CompanyId == companyId).FirstOrDefault();
                string website = _DBC.LookupWithKey("DOMAIN");
                string portal = _DBC.LookupWithKey("PORTAL");
                string valdiateURL = _DBC.LookupWithKey("EMAIL_VALIDATE_URL");
                string hostname = _DBC.LookupWithKey("SMTPHOST");
                string fromadd = _DBC.LookupWithKey("EMAILFROM");
                string sso_login = _DBC.GetCompanyParameter("AAD_SSO_TENANT_ID", companyId);

                string verifylink = portal + valdiateURL + companyId + "/" + guid;
                string companyLogo = portal + "/uploads/" + companyInfo.CompanyId + "/companylogos/" + companyInfo.CompanyLogoPath;

                if (string.IsNullOrEmpty(companyInfo.CompanyLogoPath))
                {
                    companyLogo = _DBC.LookupWithKey("CCLOGO");
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

                    messagebody = messagebody.Replace("{TWITTER_LINK}", _DBC.LookupWithKey("CC_TWITTER_PAGE"));
                    messagebody = messagebody.Replace("{TWITTER_ICON}", _DBC.LookupWithKey("CC_TWITTER_ICON"));
                    messagebody = messagebody.Replace("{FACEBOOK_LINK}", _DBC.LookupWithKey("CC_FB_PAGE"));
                    messagebody = messagebody.Replace("{FACEBOOK_ICON}", _DBC.LookupWithKey("CC_FB_ICON"));
                    messagebody = messagebody.Replace("{LINKEDIN_LINK}", _DBC.LookupWithKey("CC_LINKEDIN_PAGE"));
                    messagebody = messagebody.Replace("{LINKEDIN_ICON}", _DBC.LookupWithKey("CC_LINKEDIN_ICON"));

                    string[] toEmails = { emailId };

                    bool ismailsend = Email(toEmails, messagebody, fromadd, hostname, subject);
                }
            }
            catch (Exception ex)
            {
                //ToDo:
            }
        }

        public void SendNewRegistration(Registration reg)
        {
            try
            {

                string subject = string.Empty;
                string template = "COMPANY_SIGNUP_TEMP";

                //string path = Convert.ToString(DBC.LookupWithKey("API_TEMPLATE_PATH"));

                var sysparms = (from SP in _context.Set<SysParameter>()
                                where SP.Name == "PORTAL" || SP.Name == "SMTPHOST" || SP.Name == "TEMP_EMAIL_VALIDATE_URL" || SP.Name == "EMAIL_VALIDATE_COMPANY_DELETE"
                                || SP.Name == "EMAILFROM" || SP.Name == "CCLOGO" || SP.Name == "DOMAIN"
                                select new { SP.Name, SP.Value }).ToList();


                string portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault()!;
                string valdiateURL = sysparms.Where(w => w.Name == "TEMP_EMAIL_VALIDATE_URL").Select(s => s.Value).FirstOrDefault()!;
                string accountDeleteURL = sysparms.Where(w => w.Name == "EMAIL_VALIDATE_COMPANY_DELETE").Select(s => s.Value).FirstOrDefault()!;

                string verifylink = portal + valdiateURL + reg.UniqueReference;

                string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault()!;
                string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault()!;

                string deleteVerifyLink = portal + accountDeleteURL + reg.UniqueReference;

                string message = _DBC.ReadHtmlFile(template, "DB", 0, out subject).ToString();
                string CCimage = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault()!;
                string domain = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault()!;

                if ((hostname != null) && (fromadd != null))
                {

                    string messagebody = message;
                    messagebody = messagebody.Replace("{RECIPIENT_NAME}", reg.FirstName + " " + reg.LastName);
                    messagebody = messagebody.Replace("{RECIPIENT_EMAIL}", reg.Email);
                    messagebody = messagebody.Replace("{PASSWORD}", reg.Password);
                    messagebody = messagebody.Replace("{DOMAIN}", domain);
                    messagebody = messagebody.Replace("{COMPANY_LOGO}", CCimage);
                    messagebody = messagebody.Replace("{CUSTOMER_ID}", reg.CustomerId);

                    messagebody = messagebody.Replace("{VERIFY_LINK}", verifylink);
                    messagebody = messagebody.Replace("{DELETE_ACCOUNT_LINK}", deleteVerifyLink);
                    messagebody = messagebody.Replace("{PORTAL}", portal);

                    string[] toEmails = { reg.Email };
                    bool ismailsend = Email(toEmails, messagebody, fromadd, hostname, subject);
                }
            }
            catch (Exception ex)
            {
                //ToDo: throwExcption
            }
        }

        private static string GetEmailTemplate(string URLTemplate)
        {
            string message = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URLTemplate);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                message = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }
            return message;
        }

        public void KeyContactEmail(string tmpltype, int userId, int inciActId, string statusReason = "")
        {
            try
            {
                var userInfo = (from U in _context.Set<User>()
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
                                }).FirstOrDefault();

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
                    string message = Convert.ToString(_DBC.ReadHtmlFile(template, "DB", userInfo.CompanyID, out Subject));


                    string hostname = _DBC.LookupWithKey("SMTPHOST");
                    string fromadd = _DBC.LookupWithKey("EMAILFROM");
                    if ((message != null) && (hostname != null) && (fromadd != null))
                    {
                        string messagebody = message;

                        var InciRow = (from IA in _context.Set<Core.Models.IncidentActivation>()
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
                                           LaunchedBy =  _context.Set<User>().Where(u => u.UserId == IA.LaunchedBy).Select(a => new UserFullName { Firstname = a.FirstName, Lastname = a.LastName }).FirstOrDefault(),
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
                            actionby = _DBC.UserName(InciRow.ActivatedBy);
                            inidate = InciRow.ActivatedOn.ToString();
                        }
                        else if (tmpltype == "deactivate")
                        {
                            actionby = _DBC.UserName(InciRow.DeactivatedBy);
                            inidate = InciRow.DeactivatedOn.ToString();
                        }
                        else if (tmpltype == "cancel")
                        {
                            actionby = _DBC.UserName(InciRow.UpdatedBy);
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

                        var sysparms = (from SP in _context.Set<SysParameter>()
                                        where SP.Name == "CC_TWITTER_PAGE" || SP.Name == "CC_FB_PAGE"
                                        || SP.Name == "CC_LINKEDIN_PAGE" || SP.Name == "DOMAIN"
                                        || SP.Name == "CC_TWITTER_ICON" || SP.Name == "CC_FB_ICON"
                                        || SP.Name == "CC_LINKEDIN_ICON" || SP.Name == "CC_USER_SUPPORT_LINK"
                                        || SP.Name == "PORTAL" || SP.Name == "CCLOGO"
                                        select new { SP.Name, SP.Value }).ToList();

                        twiterlink = sysparms.Where(w => w.Name == "CC_TWITTER_PAGE").Select(s => s.Value).FirstOrDefault()!;
                        facebooklink = sysparms.Where(w => w.Name == "CC_FB_PAGE").Select(s => s.Value).FirstOrDefault()!;
                        linkedinlink = sysparms.Where(w => w.Name == "CC_LINKEDIN_PAGE").Select(s => s.Value).FirstOrDefault()!;
                        domain = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault()!;
                        twitericon = sysparms.Where(w => w.Name == "CC_TWITTER_ICON").Select(s => s.Value).FirstOrDefault()!;
                        facebookicon = sysparms.Where(w => w.Name == "CC_FB_ICON").Select(s => s.Value).FirstOrDefault()!;
                        linkedinicon = sysparms.Where(w => w.Name == "CC_LINKEDIN_ICON").Select(s => s.Value).FirstOrDefault()!;
                        usersupportlink = sysparms.Where(w => w.Name == "CC_USER_SUPPORT_LINK").Select(s => s.Value).FirstOrDefault()!;
                        LoginLink = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault()!;


                        string CompanyLogo = LoginLink + "/uploads/" + userInfo.CompanyID + "/companylogos/" + userInfo.CompanyLogo;
                        if (string.IsNullOrEmpty(userInfo.CompanyLogo))
                        {
                            CompanyLogo = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault()!;
                        }


                        messagebody = messagebody.Replace("{RECIPIENT_NAME}", _DBC.UserName(userInfo.Username));
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
                        bool ismailsend = Email(toEmails, messagebody, fromadd, hostname, Subject);
                    }
                }
            }
            catch (Exception ex)
            {
                //ToDo throw exception
            }
        }

        public void NotifyKeyHolders(string tmpltype, int inciActId, int currentUserid, int companyId, string statusReason = "", bool hasNominatedKH = false)
        {
            try
            {

                var roles = _DBC.CCRoles(true);
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
                    KeyContactEmail(tmpltype, user.UserId, inciActId, statusReason);
                    userToNotify.Add(user.UserId);
                }

                bool sendPing = Convert.ToBoolean(_DBC.GetCompanyParameter("NOTIFY_KEYHOLDER_BY_PING", companyId));

                //Making the http call to send a ping to all key holders.

                if (sendPing)
                {
                    string APIBaseURL = _DBC.LookupWithKey("APIBASEURL");

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(APIBaseURL + _DBC.getapiversion());

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    string messageText = string.Empty;

                    string portalUrl = _DBC.LookupWithKey("PORTAL");

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

                    string userpwd = _context.Set<User>().Where(w => w.UserId == currentUserid).Select(s => s.Password).FirstOrDefault();
                    userpwd = _DBC.PWDencrypt(userpwd);

                    string commsMethod = _DBC.GetCompanyParameter("DEFAULT_AWAITING_INCIDENT_CHANNEL", companyId);
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

        public bool Email(string[] toAddress, string messageBody, string fromAddress, string provider, string Subject, System.Net.Mail.Attachment fileattached = null)
        {
            try
            {
                bool emailstatus = false;

                if (provider.ToUpper() == "OFFICE365")
                {
                    string Office365Host = _DBC.LookupWithKey("AWS_SMTP_HOST");
                    emailstatus = Office365(toAddress, messageBody, fromAddress, Office365Host, Subject, fileattached);
                    //} else if(Provider.ToUpper() == "SENDGRID") {
                    //    emailstatus = SendGridEmail(ToAddress, MessageBody, FromAddress, SendGridAPIKey, Subject);
                }
                else if (provider.ToUpper() == "AWSSES")
                {
                    emailstatus = AmazonSESEmail(toAddress, messageBody, fromAddress, Subject, fileattached);
                }

                return emailstatus;
            }
            catch (Exception ex)
            {
                throw;
                //ToDo : throw exception
            }
        }

        public bool Office365(string[] toAddress, string messageBody, string fromAddress, string host, string subject, System.Net.Mail.Attachment fileattached = null)
        {
            try
            {

                MailAddress from = new MailAddress(fromAddress);

                MailMessage Message = new MailMessage();
                Message.From = from;

                foreach (string toaddress in toAddress)
                    Message.To.Add(new MailAddress(toaddress));

                if (fileattached != null)
                {
                    Message.Attachments.Add(fileattached);
                }

                Message.Subject = subject;
                Message.Body = messageBody;
                Message.BodyEncoding = Encoding.UTF8;
                Message.IsBodyHtml = true;
                Message.Priority = MailPriority.High;
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Host = host;
                smtpClient.Send(Message);
                return true;
            }
            catch (Exception ex)
            {
                throw;
                //To Do: Throw exception
            }
        }

        public bool AmazonSESEmail(string[] toAddress, string messageBody, string fromAddress, string subject, System.Net.Mail.Attachment fileattached = null)
        {
            try
            {
                string AWSSESHost = _DBC.LookupWithKey("AWS_SMTP_HOST");
                string AWSSESUser = _DBC.LookupWithKey("AWS_SMTP_USER");
                string AWSSESPwd = _DBC.LookupWithKey("AWS_SMTP_PWD");
                int AWSSESPort = Convert.ToInt32(_DBC.LookupWithKey("AWS_SMTP_PORT"));
                bool AWSSESSSL = Convert.ToBoolean(_DBC.LookupWithKey("AWS_SMTP_SSL"));

                MailAddress from = new MailAddress(fromAddress);

                MailMessage Message = new MailMessage();
                Message.From = from;

                foreach (string toaddress in toAddress)
                    Message.To.Add(new MailAddress(toaddress));

                if (fileattached != null)
                {
                    Message.Attachments.Add(fileattached);
                }

                Message.Subject = subject;
                Message.Body = messageBody;
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
                //To Do: Throw exception
                throw;

            }
        }

        public bool ServiceJobExecution(string emailType, string jobKey, string jobName, string failureEmailList, int companyid, string strSubject = "", string message = "", System.Net.Mail.Attachment fileattached = null)
        {
            try
            {
                string filename = string.Empty;
                bool emailStus = false;
                string Subject = string.Empty;
                if (emailType.ToUpper().Trim() == "SUCCESS")
                {
                    filename = "TRIGGER_SUCCESS";
                }
                else if (emailType.ToUpper().Trim() == "FAILED")
                {
                    filename = "TRIGGER_FAILED";
                }
                else if (emailType.ToUpper().Trim() == "JOBKEYNOTMATCHED")
                {
                    filename = "TRIGGER_NOT_FOUND";
                }
                else if (emailType.ToUpper().Trim() == "JOBKEYNOTENABLE")
                {
                    filename = "TRIGGER_NOT_ENABLED";
                }
                else if (emailType.ToUpper().Trim() == "SOURCEEMILNOTMATCHED")
                {
                    filename = "SOURCE_EMAIL_NOT_MATCHED";
                }
                else if (emailType.ToUpper().Trim() == "IMPORTJOBALERT")
                {
                    filename = "IMPORT_JOB_ALERT";
                }
                if (!string.IsNullOrEmpty(filename))
                {
                    //    TemplateURL = path + filename;

                    var companyInfo = _context.Set<Company>().Where(c=> c.CompanyId == companyid).FirstOrDefault();

                    string domain = string.Empty;
                    string twiterlink = string.Empty;
                    string twitericon = string.Empty;
                    string linkedinlink = string.Empty;
                    string linkedinicon = string.Empty;
                    string facebooklink = string.Empty;
                    string facebookicon = string.Empty;
                    string usersupportlink = string.Empty;
                    string hostName = string.Empty;
                    string emailFrom = string.Empty;
                    string CCimage = string.Empty;
                    string portal = string.Empty;

                    var sysparms = (from SP in _context.Set<SysParameter>()
                                    where SP.Name == "CC_TWITTER_PAGE" || SP.Name == "CC_FB_PAGE"
                                    || SP.Name == "CC_LINKEDIN_PAGE" || SP.Name == "DOMAIN"
                                    || SP.Name == "CC_TWITTER_ICON" || SP.Name == "CC_FB_ICON"
                                    || SP.Name == "CC_LINKEDIN_ICON" || SP.Name == "CC_USER_SUPPORT_LINK"
                                    || SP.Name == "SMTPHOST" || SP.Name == "EMAILFROM"
                                    || SP.Name == "CCLOGO" || SP.Name == "PORTAL"
                                    select new { SP.Name, SP.Value }).ToList();

                    twiterlink = sysparms.Where(w => w.Name == "CC_TWITTER_PAGE").Select(s => s.Value).FirstOrDefault();
                    facebooklink = sysparms.Where(w => w.Name == "CC_FB_PAGE").Select(s => s.Value).FirstOrDefault();
                    linkedinlink = sysparms.Where(w => w.Name == "CC_LINKEDIN_PAGE").Select(s => s.Value).FirstOrDefault();
                    domain = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault();
                    twitericon = sysparms.Where(w => w.Name == "CC_TWITTER_ICON").Select(s => s.Value).FirstOrDefault();
                    facebookicon = sysparms.Where(w => w.Name == "CC_FB_ICON").Select(s => s.Value).FirstOrDefault();
                    linkedinicon = sysparms.Where(w => w.Name == "CC_LINKEDIN_ICON").Select(s => s.Value).FirstOrDefault();
                    usersupportlink = sysparms.Where(w => w.Name == "CC_USER_SUPPORT_LINK").Select(s => s.Value).FirstOrDefault();
                    hostName = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault();
                    emailFrom = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault();
                    CCimage = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault();
                    portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault();


                    string messagebody = Convert.ToString(_DBC.ReadHtmlFile(filename, "DB", companyid, out Subject));
                    Subject = !string.IsNullOrEmpty(strSubject) ? strSubject : Subject;

                    string CompanyLogo = portal + "/uploads/" + companyInfo.CompanyId + "/companylogos/" + companyInfo.CompanyLogoPath;
                    if (string.IsNullOrEmpty(companyInfo.CompanyLogoPath))
                    {
                        CompanyLogo = CCimage;
                    }

                    if ((messagebody != null) && (hostName != null) && (emailFrom != null))
                    {
                        messagebody = messagebody.Replace("{TWITTER_LINK}", twiterlink);
                        messagebody = messagebody.Replace("{TWITTER_ICON}", twitericon);
                        messagebody = messagebody.Replace("{FACEBOOK_LINK}", facebooklink);
                        messagebody = messagebody.Replace("{FACEBOOK_ICON}", facebookicon);
                        messagebody = messagebody.Replace("{LINKEDIN_LINK}", linkedinlink);
                        messagebody = messagebody.Replace("{LINKEDIN_ICON}", linkedinicon);
                        messagebody = messagebody.Replace("{CC_WEBSITE}", domain);
                        messagebody = messagebody.Replace("{CC_USER_SUPPORT_LINK}", usersupportlink);
                        messagebody = messagebody.Replace("{COMPANY_NAME}", companyInfo.CompanyName);
                        messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                        messagebody = messagebody.Replace("{TRIGGER_JOB_NAME}", jobName);
                        messagebody = messagebody.Replace("{TRIGGER_KEY}", jobKey);
                        messagebody = messagebody.Replace("{CC_LOGO}", CCimage);
                        messagebody = messagebody.Replace("{TRIGGER_JOB_MESSAGE}", message);
                        messagebody = messagebody.Replace("{CUSTOMER_ID}", companyInfo.CustomerId);

                        string[] toAddress = failureEmailList.Split(new char[] { ';', ',' });

                        emailStus = Email(toAddress, messagebody, emailFrom, hostName, Subject, fileattached);
                    }
                }
                return emailStus;
            }
            catch (Exception ex)
            {
                throw;
                //To Do: throw exception
            }
        }

        public void SendMonthlyPaymentAlert(int companyId, decimal totalMonthlyDebitAmount, decimal totalAmountDebited, decimal vatAmount, string email_items)
        {
            try
            {

                //string path = Convert.ToString(DBC.LookupWithKey("API_TEMPLATE_PATH")) + "MonthlyPaymentSuccess.html";
                string Subject = string.Empty;
                string message = Convert.ToString(_DBC.ReadHtmlFile("MONTHLY_PAYMENT_SUCCESS", "DB", companyId, out Subject));
                if (!string.IsNullOrEmpty(message))
                {


                    var company = (from C in _context.Set<Company>()
                                   join CP in _context.Set<CompanyPaymentProfile>() on C.CompanyId equals CP.CompanyId
                                   where C.CompanyId == companyId
                                   select new { C, CP }).FirstOrDefault();
                    if (company != null)
                    {


                        var sysparms = (from SP in _context.Set<SysParameter>()
                                        where SP.Name == "PORTAL" || SP.Name == "SMTPHOST"
                                        || SP.Name == "EMAILFROM" || SP.Name == "CCLOGO"
                                        select new { SP.Name, SP.Value }).ToList();

                        string Portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault()!;
                        string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault()!;
                        string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault()!;

                        string CompanyLogo = Portal + "/uploads/" + company.C.CompanyId + "/companylogos/" + company.C.CompanyLogoPath;
                        if (string.IsNullOrEmpty(company.C.CompanyLogoPath))
                        {
                            CompanyLogo = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault()!;
                        }

                        if ((message != null) && (hostname != null) && (fromadd != null))
                        {
                            string messagebody = message;

                            string billing_email = _DBC.LookupWithKey("BILLING_EMAIL");

                            //Get company billing email list.
                            string billing_users = _DBC.GetCompanyParameter("BILLING_USERS", company.C.CompanyId);

                            List<string> emaillist = new List<string>();

                            if (!string.IsNullOrEmpty(billing_users))
                            {
                                var user_ids = billing_users.Split(',').Select(int.Parse).ToList();
                                if (user_ids.Count > 0)
                                {
                                    var get_user = (from U in _context.Set<User>()
                                                    where user_ids.Contains(U.UserId) && U.Status != 3
                                                    select new
                                                    {
                                                        U.PrimaryEmail
                                                    }).ToList();
                                    foreach (var bill_user in get_user)
                                    {
                                        emaillist.Add(bill_user.PrimaryEmail);
                                    }
                                }
                            }

                            email_items = email_items.Replace("{TOTAL_PAYMENT_WITH_VAT}", _DBC.ToCurrency(totalMonthlyDebitAmount));
                            email_items = email_items.Replace("{VAT_VALUE}", _DBC.ToCurrency(vatAmount));

                            messagebody = messagebody.Replace("{COMPANY_NAME}", company.C.CompanyName);
                            messagebody = messagebody.Replace("{CUSTOMER_ID}", company.C.CustomerId);
                            messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                            messagebody = messagebody.Replace("{PORTAL}", Portal);

                            messagebody = messagebody.Replace("{BILLING_EMAIL}", billing_email);
                            messagebody = messagebody.Replace("{TOTAL_PAYMENT_WITH_VAT}", _DBC.ToCurrency(totalMonthlyDebitAmount));
                            messagebody = messagebody.Replace("{VAT_VALUE}", _DBC.ToCurrency(vatAmount));
                            messagebody = messagebody.Replace("{PAYMENT_COLLECTED}", _DBC.ToCurrency(totalAmountDebited));
                            messagebody = messagebody.Replace("{TRANSACTION_ITEMS}", email_items);

                            string[] toEmails = emaillist.ToArray();
                            string[] adm_email = { billing_email };
                            Email(adm_email, messagebody, fromadd, hostname, Subject);
                            Email(toEmails, messagebody, fromadd, hostname, Subject);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
                //TODO throw exception
            }
        }
        public void SendUserAssociationsToAdmin(string items, int userId, int companyId)
        {
            try
            {
                var roles = _DBC.CCRoles();
                var user =  _context.Set<User>().Where(t => t.UserId == userId).Select(t => new UserFullName { Firstname = t.FirstName, Lastname = t.LastName }).FirstOrDefault();

                var adminuser = (from U in _context.Set<User>()
                                 where roles.Contains(U.UserRole) && U.CompanyId == companyId && U.Status == 1
                                 select new { U.PrimaryEmail, U.FirstName, U.LastName }).ToList();

                var company = (from C in _context.Set<Company>()
                               join CP in _context.Set<CompanyPaymentProfile>() on C.CompanyId equals CP.CompanyId
                               where C.CompanyId == companyId
                               select new { C, CP }).FirstOrDefault();
                if (company != null)
                {
                    string templatename = "USER_DELETE_ALERT";
                    string Subject = string.Empty;
                    string message = Convert.ToString(_DBC.ReadHtmlFile(templatename, "DB", company.C.CompanyId, out Subject));

                    var sysparms = (from SP in _context.Set<SysParameter>()
                                    where SP.Name == "PORTAL" || SP.Name == "SMTPHOST"
                                    || SP.Name == "EMAILFROM" || SP.Name == "CCLOGO"
                                    select new { SP.Name, SP.Value }).ToList();

                    string Website = sysparms.Where(w => w.Name == "DOMAIN").Select(s => s.Value).FirstOrDefault();
                    string Portal = sysparms.Where(w => w.Name == "PORTAL").Select(s => s.Value).FirstOrDefault();

                    string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault();
                    string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault();
                    string CompanyLogo = Portal + "/uploads/" + company.C.CompanyId + "/companylogos/" + company.C.CompanyLogoPath;

                    if (string.IsNullOrEmpty(company.C.CompanyLogoPath))
                    {
                        CompanyLogo = sysparms.Where(w => w.Name == "CCLOGO").Select(s => s.Value).FirstOrDefault();
                    }

                    if ((message != null) && (hostname != null) && (fromadd != null))
                    {
                        string messagebody = message;

                        messagebody = messagebody.Replace("{COMPANY_NAME}", company.C.CompanyName);
                        messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                        messagebody = messagebody.Replace("{CC_WEBSITE}", Website);
                        messagebody = messagebody.Replace("{PORTAL}", Portal);
                        messagebody = messagebody.Replace("{DELETED_USER_NAME}", user.Firstname + " " + user.Lastname);
                        messagebody = messagebody.Replace("{USER_LINKS}", items);
                        messagebody = messagebody.Replace("{CUSTOMER_ID}", company.C.CustomerId);

                        foreach (var admin in adminuser)
                        {
                            string sendbody = messagebody;
                            sendbody = sendbody.Replace("{RECIPIENT_NAME}", admin.FirstName + " " + admin.LastName);
                            string[] adm_email = { admin.PrimaryEmail };
                            Email(adm_email, sendbody, fromadd, hostname, Subject);
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

    }
}
