using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Models;
using CrisesControl.Core.Reports.SP_Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports.Repositories {
    public interface IReportsRepository {
        public Task<List<SOSItem>> GetSOSItems();
        public Task<List<IncidentPingStatsCount>> GetIncidentPingStats(int CompanyID, int NoOfMonth);
        Task<List<ResponseSummary>> ResponseSummary(int MessageID);
        Task<List<MessageAcknowledgements>> GetIndidentMessageAck(int MessageId, int MessageAckStatus, int MessageSentStatus, int RecordStart, int RecordLength, string SearchString,
            string OrderBy, string OrderDir, int CurrentUserId, string Filters, string CompanyKey, string Source);
        Task<List<DataTablePaging>> GetIndidentMessageNoAck(int draw, int IncidentActivationId
        ,int RecordStart, int RecordLength, string? SearchString, string? UniqueKey);
        GetCurrentIncidentStatsResponse GetCurrentIncidentStats(string timeZoneId);
        IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId);
    }
}
