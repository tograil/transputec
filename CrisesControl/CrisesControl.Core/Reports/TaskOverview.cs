using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class TaskOverview
    {
        public int Accepted { get; set; }
        public int NotAccepted { get; set; }
        public int PendingComplete { get; set; }
        public int Completed { get; set; }
        public int PendingPredecessor { get; set; }
        public int Total { get; set; }
    }
}
