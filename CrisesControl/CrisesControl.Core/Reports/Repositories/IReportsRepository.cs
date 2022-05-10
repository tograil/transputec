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
    }
}
