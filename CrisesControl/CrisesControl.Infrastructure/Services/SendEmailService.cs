using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class SendEmailService : ISenderEmailService
    {
        private readonly CrisesControlContext _context;
        public SendEmailService(CrisesControlContext context)
        {
            this._context=context;
        }
        public async Task<bool> Email(string[] ToAddress, string MessageBody, string FromAddress, string Provider, string Subject, System.Net.Mail.Attachment fileattached = null)
        {
            try
            {
                bool emailstatus = false;

                if (Provider.ToUpper() == "OFFICE365")
                {
                    string Office365Host = LookupWithKey("AWS_SMTP_HOST");
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
                string AWSSESHost =  LookupWithKey("AWS_SMTP_HOST");
                string AWSSESUser =  LookupWithKey("AWS_SMTP_USER");
                string AWSSESPwd =  LookupWithKey("AWS_SMTP_PWD");
                int AWSSESPort = Convert.ToInt32( LookupWithKey("AWS_SMTP_PORT"));
                bool AWSSESSSL = Convert.ToBoolean(LookupWithKey("AWS_SMTP_SSL"));

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
        public async Task RegistrationCancelled(string CompanyName, int PlanId, DateTimeOffset RegDate, UserFullName pUserName, string pUserEmail, PhoneNumber pUserMobile)
        {
            var sysparms =await  _context.Set<SysParameter>()
                            .Where(SP=> SP.Name == "PORTAL" || SP.Name == "SMTPHOST" || SP.Name == "EMAILFROM")
                            .Select(SP => new { SP.Name, SP.Value }).ToListAsync();

            string hostname = sysparms.Where(w => w.Name == "SMTPHOST").Select(s => s.Value).FirstOrDefault();
            string fromadd = sysparms.Where(w => w.Name == "EMAILFROM").Select(s => s.Value).FirstOrDefault();

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

    }
}
