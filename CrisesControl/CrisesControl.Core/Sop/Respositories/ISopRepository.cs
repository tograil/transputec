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
        Task<int> AU_SOPHeader(int SOPHeaderID, int IncidentID, string SOPVersion, int SOPOwner, string BriefDescription, DateTimeOffset NextReviewDate, string ReviewFrequency,
                                int ContentID, int ContentSectionID, int CurrentUserID, int CompanyId, int Status = 1, string TimeZoneId = "GMT Standard Time");
        Task<List<ContentTags>> GetContentTags();
        Task<int> AU_SOPDetail(int SOPHeaderID, int ContentID, int ContentSectionID);
        Task CreateContentVersion(int PrimaryContentID, int LastContentID, int CurrentUserID, string TimeZoneId = "GMT Standard Time");
        Task<int> AU_Content(int ContentID, string ContentText, string ContentType, int Status, int CurrentUserID, string TimeZoneId = "GMT Standard Time");
        Task<int> AU_ContentSection(int ContentSectionID, int SOPHeaderID, string SectionName, int Status, string SectionType, int SectionOrder, int CurrentUserID, string TimeZoneId = "GMT Standard Time");
        Task<ContentSectionData> GetSOPSections(int SOPHeaderID, int CompanyID, int ContentSectionID = 0);
        Task<int> AU_Section(int SOPHeaderID, int ContentID, int ContentSectionID, string SectionType, string SectionName, string SectionDescription, int SectionStatus, int SectionOrder,
            List<int> Groups, List<int> Tags, int CurrentUserId);
        Task DeleteSections(int ContentSectionID);
        Task AU_SOPGroupDetail(int SOPDetailID, List<int> Groups, int CurrentUserID, string TimeZoneId = "GMT Standard Time");
        Task AU_ContentTag(int ContentID, List<int> Tags, int CurrentUserID, string TimeZoneId = "GMT Standard Time");
        Task<int> ReorderSection(List<Section> SectionOrder);
    }
}
