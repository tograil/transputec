using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Sop.Respositories
{
    public interface ISopRepository
    {
        Task<List<LibSopSection>> GetSOPSectionLibrary();
        Task<GetSopResponse> GetSOP(int CompanyId, int SOPHeaderID = 0);
        Task<int> SaveSOPHeader(Sopheader SOPHeader);
        Task<int> UPdateSOPHeader(Sopheader SOPHeader);
        Task LinkSOPWithIncident(int SOPHeaderID, int IncidentID, int CurrentUserID, string TimeZoneId);
        Task<Sopheader> GetSopheaderById(int SOPHeaderID);
        Task<Profit> GetCompanyProfile(int CompanyId);
        Task<int> GetTransactionTypeID(string TransactionCode);
        Task CreateSOPReviewReminder(int IncidentID, int SOPHeaderID, int CompanyID, DateTimeOffset NextReviewDate, string ReviewFrequency, int ReminderCount);
        Task<int> UpdateTransactionDetails(int TransactionHeaderId, int CompanyId, int TransactionTypeId, decimal TransactionRate, decimal MinimumPrice,
           int Quantity, decimal Cost, decimal LineValue, decimal LineVAT, decimal Total, int MessageId, DateTimeOffset TransactionDate, int currntUserId = 0,
           string TransactionReference = "", string TimeZoneId = "GMT Standard Time", int TransactionStatus = 1, int TransactionDetailsId = 0, string TrType = "DR");
    }
}
