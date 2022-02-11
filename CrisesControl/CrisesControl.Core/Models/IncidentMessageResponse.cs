namespace CrisesControl.Core.Models
{
    public partial class IncidentMessageResponse
    {
        public int IncidentResponseId { get; set; }
        public int? IncidentId { get; set; }
        public int? ResponseId { get; set; }
        public int ResponseCode { get; set; }
    }
}
