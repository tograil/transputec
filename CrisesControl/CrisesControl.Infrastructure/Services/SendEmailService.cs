using CrisesControl.Core.Models;
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
                    string Office365Host = await LookupWithKey("AWS_SMTP_HOST");
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
                string AWSSESHost = await LookupWithKey("AWS_SMTP_HOST");
                string AWSSESUser = await LookupWithKey("AWS_SMTP_USER");
                string AWSSESPwd = await LookupWithKey("AWS_SMTP_PWD");
                int AWSSESPort = Convert.ToInt32(await LookupWithKey("AWS_SMTP_PORT"));
                bool AWSSESSSL = Convert.ToBoolean(await LookupWithKey("AWS_SMTP_SSL"));

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
        private async Task<string> LookupWithKey(string Key, string Default = "")
        {
            try
            {
                var LKP = await _context.Set<SysParameter>()
                          .Where(L=> L.Name == Key).FirstOrDefaultAsync();
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

    }
}
