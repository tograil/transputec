namespace CrisesControl.Core.Models
{
    public partial class IncidentParticipant
    {
        public int IncidentParticipantId { get; set; }
        public int IncidentId { get; set; }
        public int? IncidentActionId { get; set; }
        public string ParticipantType { get; set; } = null!;
        public int? ParticipantGroupId { get; set; }
        public int ObjectMappingId { get; set; }
        public int? ParticipantUserId { get; set; }
    }
}
