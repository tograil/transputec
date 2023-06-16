using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Users
{
    public class OffDutyModel
    {
        public string OffDutyAction { get; set; }
        public DateTimeOffset StartDateTime { get; set; }
        public DateTimeOffset EndDateTime { get; set; }
        public bool AllowPush { get; set; }
        public bool AllowPhone { get; set; }
        public bool AllowText { get; set; }
        public bool AllowEmail { get; set; }
        public string Source { get; set; }

    }
}
