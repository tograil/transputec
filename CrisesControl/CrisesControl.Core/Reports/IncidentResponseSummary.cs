using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class IncidentResponseSummary
    {
        public string MessageText { get; set; }
        public DateTimeOffset DateSent { get; set; }
        public int MessageId { get; set; }
        public string ResponseLabel { get; set; }
        public int ResponseCount { get; set; }
    }
}
