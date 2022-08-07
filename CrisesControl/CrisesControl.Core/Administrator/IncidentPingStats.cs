using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Administrator
{
    public class IncidentPingStats
    {
        public int CompanyId { get; set; }
        public Nullable<int> TotalPingInKPI { get; set; }
        public Nullable<int> TotalPingOutKPI { get; set; }
        public Nullable<int> TotalPingOutKPIMax { get; set; }
        public Nullable<int> TotalPingACK { get; set; }
        public Nullable<int> TotalPingSent { get; set; }
        public Nullable<int> TotalIncidentInKPI { get; set; }
        public Nullable<int> TotalIncidentOutKPI { get; set; }
        public Nullable<int> TotalIncidentOutKPIMax { get; set; }
        public Nullable<int> TotalIncidentACK { get; set; }
        public Nullable<int> TotalIncidentSent { get; set; }
        public Nullable<int> TotalSent { get; set; }
        public Nullable<long> TotalCreated { get; set; }
    }
}
