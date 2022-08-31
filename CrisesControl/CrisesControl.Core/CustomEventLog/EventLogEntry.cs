using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CustomEventLog
{
    public class EventLogEntry: EventLogModel
    {
        public int EventLogID { get; set; }
        public DateTimeOffset LogEntryDateTime { get; set; }
        public string IncidentDetails { get; set; }
        public string SourceOfInformation { get; set; }
        public Nullable<int> IsConfirmed { get; set; }
        public string CMTAction { get; set; }
        public Nullable<int> ActionPriority { get; set; }
        public Nullable<int> ActionUser { get; set; }
        public Nullable<int> ActionGroup { get; set; }
        public Nullable<DateTimeOffset> ActionDueBy { get; set; }
        public Nullable<int> StatusOfAction { get; set; }
        public string ActionDetail { get; set; }
        public string Comments { get; set; }
        public Nullable<DateTimeOffset> ActionedDate { get; set; }
    }
}
