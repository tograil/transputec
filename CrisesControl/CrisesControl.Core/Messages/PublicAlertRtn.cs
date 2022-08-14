using CrisesControl.Core.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Messages
{
    public class PublicAlertRtn
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public bool Scheduled { get; set; }
        public DateTimeOffset ScheduleAt { get; set; }
        public int Executed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserFullName SentBy { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
