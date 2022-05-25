using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Models;
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
        Task<List<PingGroupChartCount>> GetPingReportChart(DateTime StartDate, DateTime EndDate, int GroupID, string MessageType, int ObjectMappingID);
        Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "");
    }
}
