using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports.SP_Response
{
    public class CurrentIncidentStatsResponse
    {
        public int ActivatedCount { get; set; }
        public int LaunchedCount { get; set; }
        public int DeactivatedCount { get; set; }
        public int ClosedCount { get; set; }
        public int ActMonthCount { get; set; }
        public int LnchdMonthCount { get; set; }
        public int DeactMonthCount { get; set; }
        public int ClosedMonthCount { get; set; }
    }
}
