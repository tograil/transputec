using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.App
{
    public class AppHomeRatings
    {
        public int CompanyId { get; set; }
        public int RecepientUserId { get; set; }
        public Nullable<int> TotalPingInKPI { get; set; }
        public Nullable<int> TotalPingACK { get; set; }
        public Nullable<int> TotalPingSent { get; set; }
        public Nullable<int> TotalIncidentInKPI { get; set; }
        public Nullable<int> TotalIncidentACK { get; set; }
        public Nullable<int> TotalIncidentSent { get; set; }
        public Nullable<int> TotalIncidentUnAck { get; set; }
        public Nullable<int> TotalSent { get; set; }
        public Nullable<long> TotalCreated { get; set; }
        public int TaskCount { get; set; }
        public int PendingTask { get; set; }
        public int ActiveCompletedTask { get; set; }
    }
}
