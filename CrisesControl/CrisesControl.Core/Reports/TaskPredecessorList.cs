using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class TaskPredecessorList
    {
        public int PredecessorTaskID { get; set; }
        public int TaskPredecessorID { get; set; }
        public int TaskSequence { get; set; }
        public string TaskTitle { get; set; }
        public int TaskStatus { get; set; }
    }
}
