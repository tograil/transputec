namespace CrisesControl.Core.Messages
{
    public partial class MessageMethod
    {
        public int MessageMethhodId { get; set; }
        public int MethodId { get; set; }
        public int MessageId { get; set; }
        public int ActiveIncidentId { get; set; }
        public int? IncidentId { get; set; }
    }
}
