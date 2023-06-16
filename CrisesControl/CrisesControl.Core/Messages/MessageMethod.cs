using CrisesControl.Core.Models;

namespace CrisesControl.Core.Messages
{
    public class MessageMethod
    {
        public int MessageMethhodId { get; set; }
        public int MethodId { get; set; }
        public int MessageId { get; set; }
        public int ActiveIncidentId { get; set; }
        public int? IncidentId { get; set; }
        public CommsMethod CommsMethod { get; set; }
    }
}
