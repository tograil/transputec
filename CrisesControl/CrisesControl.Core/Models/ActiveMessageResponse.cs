namespace CrisesControl.Core.Models
{
    public partial class ActiveMessageResponse
    {
        public int MessageResponseId { get; set; }
        public int MessageId { get; set; }
        public int ResponseId { get; set; }
        public string? ResponseLabel { get; set; }
        public int ResponseCode { get; set; }
        public bool IsSafetyResponse { get; set; }
        public string? SafetyAckAction { get; set; }
        public int? ActiveIncidentId { get; set; }
    }
}
