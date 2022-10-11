using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.SopLibrary.Repositories
{
    public interface ISopLibraryRepository
    {
        Task<int> AU_Section(int SOPHeaderID, int IncidentID, string SOPVersion, DateTimeOffset NextReviewDate,
           int ContentID, int ContentSectionID, string SectionName, string SectionDescription, int SectionStatus, List<int> Tags, int CurrentUserId, int CompanyID, string TimeZoneId);
        Task<int> AU_LibSOPHeader(int SOPHeaderID, int IncidentID, string SOPVersion, DateTimeOffset NextReviewDate, int currentUserId, int CompanyID, int Status = 1, string TimeZoneId = "GMT Standard Time");
        Task<int> AU_LibContentSection(int ContentSectionID, int SOPHeaderID, string SectionName, int Status);
        Task<int> AU_LibSOPDetail(int SOPHeaderID, int ContentID, int ContentSectionID);
        Task AU_LibContentTag(int ContentID, List<int> Tags, int currentUserId, string TimeZoneId);
        Task<dynamic> RecordLibraryUsage(int LibSOPHeaderID);
        Task<bool> DeleteSOPLib(int LibSOPHeaderID);
        Task<List<SopSectionList>> GetSOPLibrarySections(int CompanyID);
        Task<SopSection> GetSOPLibrarySection(int SOPHeaderID, int CompanyID);
        Task<int> AU_LibContent(int ContentID, string ContentText, int Status, int currentUserId, string TimeZoneId);
    }
}
