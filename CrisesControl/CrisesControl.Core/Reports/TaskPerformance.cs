using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class TaskPerformance
    {
        public int CountTask { get; set; }
        public int AcceptedOnTime { get; set; }
        public int NotAcceptedOnTime { get; set; }
        public int CompletedOnTime { get; set; }
        public int NotCompletedOnTime { get; set; }
        public int TotalCompleted { get; set; }
    }
}
