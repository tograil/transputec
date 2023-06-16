using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class UserPieChart
    {
        public int TotalMessage { get; set; }
        public int TotalInKPI { get; set; }
        public int TotalOutMinKPI { get; set; }
        public int TotalOutMaxKPI { get; set; }
        public int TotalNoResponse { get; set; }
        public int KPILimit { get; set; }
        public int TotalWithinCutOff { get; set; }
        public int GoldenHour { get; set; }
        public int KPIMaxLimit { get; set; }
        public int TotalIncident { get; set; }
    }
}
