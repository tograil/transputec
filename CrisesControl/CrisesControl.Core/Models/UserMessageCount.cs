namespace CrisesControl.Core.Models
{
    public partial class UserMessageCount
    {
        public long? Id { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public int TotalPingUnAck { get; set; }
        public int TotalIncidentUnAck { get; set; }
        public int TaskCount { get; set; }
        public int PendingTask { get; set; }
        public int ActiveCompletedTask { get; set; }
    }
}
