namespace CrisesControl.Core.Models
{
    public partial class IncidentPingUserRating
    {
        public int CompanyId { get; set; }
        public int RecepientUserId { get; set; }
        public int? TotalPingInKpi { get; set; }
        public int? TotalPingAck { get; set; }
        public int? TotalPingSent { get; set; }
        public int? TotalIncidentInKpi { get; set; }
        public int? TotalIncidentAck { get; set; }
        public int? TotalIncidentSent { get; set; }
        public int? TotalIncidentUnAck { get; set; }
        public int? TotalSent { get; set; }
        public long? TotalCreated { get; set; }
    }
}
