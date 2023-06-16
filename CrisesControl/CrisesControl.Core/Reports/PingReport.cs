using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class PingReport {
        public int TotalCountNoResponse { get; set; }
        public int TotalCountOutMaxKPI { get; set; }
        public int TotalWithinCutOff { get; set; }
        public int TotalGoldenHour { get; set; }
        public int TotalCountInKPI { get; set; }
        public int TotalCountDiff5 { get; set; }
        public int TotalCountDiff10 { get; set; }
        public int TotalCountDiff15 { get; set; }
        public int TotalCountDiff20 { get; set; }
        public int TotalCountDiff25 { get; set; }
        public int TotalCountDiff30 { get; set; }
        public int TotalCountDiff35 { get; set; }
        public int TotalCountDiff40 { get; set; }
        public int TotalCountDiff45 { get; set; }
        public int TotalCountDiff50 { get; set; }
        public int TotalCountDiff55 { get; set; }
        public int TotalCountDiff60 { get; set; }
        [NotMapped]
        public int TotalCountOutMinKPI { get; set; }
        [NotMapped]
        public int KPILimit { get; set; } = 0;
        [NotMapped]
        public int KPIMaxLimit { get; set; } = 0;

    }
}
