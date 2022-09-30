using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public interface ISenderEmailService
    {
        Task<bool> Email(string[] ToAddress, string MessageBody, string FromAddress, string Provider, string Subject, System.Net.Mail.Attachment fileattached = null);
        Task<bool> Office365(string[] ToAddress, string MessageBody, string FromAddress, string Host, string Subject, System.Net.Mail.Attachment fileattached = null);
        Task<bool> AmazonSESEmail(string[] ToAddress, string MessageBody, string FromAddress, string Subject, System.Net.Mail.Attachment fileattached = null);
        Task RegistrationCancelled(string CompanyName, int PlanId, DateTimeOffset RegDate, UserFullName pUserName, string pUserEmail, PhoneNumber pUserMobile);
        Task UsageAlert(int CompanyId);
        Task NotifyKeyHolders(string tmpltype, int inciActId, int currentUserid, int companyId, string statusReason = "", bool hasNominatedKH = false);
        Task KeyContactEmail(string tmpltype, int userId, int inciActId, string statusReason = "");
        Task SendUserAssociationsToAdmin(string items, int userId, int companyId);
        Task NewUserAccount(string emailId, string userName, int companyId, string guid);
        Task SendReviewAlert(int incidentId, int headerId, int companyId, string reminderType = "TASK");
        Task NewUserAccountConfirm(string emailId, string userName, string userPass, int companyId, string guid);
        Task CompanySignUpConfirm(string emailId, string userName, string mobile, string paymentMethod, string plan, string userPass, int companyId);
    }
}
