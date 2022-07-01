using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class PerformanceReport
    {
        public string MessageType { get; set; }
        public string GroupName { get; set; }
        public int NoRsp { get; set; }
        public int OutMaxKPI { get; set; }
        public int WithinCutOff { get; set; }
        public int GoldHour { get; set; }
        public int InKPI { get; set; }
    }
}
