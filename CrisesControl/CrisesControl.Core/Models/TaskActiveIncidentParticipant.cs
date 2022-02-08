namespace CrisesControl.Core.Models
{
    public partial class TaskActiveIncidentParticipant
    {
        public int ActiveIncidentTaskParticipantId { get; set; }
        public int ActiveIncidentTaskId { get; set; }
        public int ParticipantTypeId { get; set; }
        public int ParticipantGroupId { get; set; }
        public int ParticipantUserId { get; set; }
        public int PreviousParticipantTypeId { get; set; }
        public string? ActionStatus { get; set; }
    }
}
