using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Reports
{
    public class DeliveryOutput
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        public string ISDCode { get; set; }
        public string Recipient { get; set; }
        public string DeviceAddress { get; set; }
        public string MethodName { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset MessageStartTime { get; set; }
        public DateTimeOffset MessageEndTime { get; set; }
        public int TotalTimeTaken { get; set; }
        public string Status { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Attempts { get; set; }
        public int MessageListID { get; set; }
        public string PrimaryEmail { get; set; }
        public DateTimeOffset DateSent { get; set; }
    }
}
