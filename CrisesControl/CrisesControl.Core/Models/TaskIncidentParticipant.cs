namespace CrisesControl.Core.Models
{
    public partial class TaskIncidentParticipant
    {
        public int IncidentTaskParticipantId { get; set; }
        public int IncidentTaskId { get; set; }
        public int ParticipantTypeId { get; set; }
        public int ParticipantGroupId { get; set; }
        public int ParticipantUserId { get; set; }
    }
}
