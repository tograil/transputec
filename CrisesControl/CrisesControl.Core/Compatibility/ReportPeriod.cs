using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Compatibility
{
    [Obsolete("Added for compatibility with old portal")]
    public class ReportPeriod : CcBase
    {
        public bool IsThisWeek { get; set; }
        public bool IsThisMonth { get; set; }
        public bool IsLastMonth { get; set; }
        public int SelectedUserID { get; set; }

        public int FilterRelation { get; set; }
        public int ObjectMapId { get; set; }
        public int MessageId { get; set; }
        public string MessageType { get; set; }
    }
}
