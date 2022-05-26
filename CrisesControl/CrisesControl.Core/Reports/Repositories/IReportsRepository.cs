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
        Task<DataTablePaging> GetIndidentMessageAck(int MessageId, int MessageAckStatus, int MessageSentStatus, int RecordStart, int RecordLength, string search,
             string OrderBy, string OrderDir, int draw, string Filters, string CompanyKey, string Source = "WEB");
        Task<List<DataTablePaging>> GetIndidentMessageNoAck(int draw, int IncidentActivationId
        ,int RecordStart, int RecordLength, string? SearchString, string? UniqueKey);
        Task<List<PingGroupChartCount>> GetPingReportChart(DateTime StartDate, DateTime EndDate, int GroupID, string MessageType, int ObjectMappingID);
        Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "");
        Task<List<DeliveryOutput>> GetMessageDeliveryReport(DateTimeOffset StartDate, DateTimeOffset EndDate, int start, int length, string search, string OrderBy,string OrderDir, string CompanyKey);
        Task<dynamic> GetMessageDeliverySummary(int MessageID);
    }
}
