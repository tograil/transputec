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
        Task<List<DataTablePaging>> GetIndidentMessageNoAck(int draw, int IncidentActivationId
        ,int RecordStart, int RecordLength, string? SearchString, string? UniqueKey);
    }
}
