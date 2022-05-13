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
        Task<dynamic> GetIndidentMessageAck(int MessageId, int MessageAckStatus, int MessageSentStatus, int start, int length, string search,
             List<Order>? order, int draw, string Filters, string CompanyKey, string Source);
    }
}
