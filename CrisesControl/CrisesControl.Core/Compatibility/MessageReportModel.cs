using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Compatibility
{
    [Obsolete("Added for compatibility with old portal")]
    public class MessageReportModel : ReportPeriod
    {
        public int MessageId { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageSentStatus { get; set; }
        public string MessageType { get; set; }
        public int DrillOpt { get; set; }
        public int GroupId { get; set; }
        public int ObjectMappingId { get; set; }
        public int FilterUser { get; set; }
        public string GroupName { get; set; }
        public string GroupType { get; set; }
        public bool ShowDeletedGroups { get; set; }
    }
}
