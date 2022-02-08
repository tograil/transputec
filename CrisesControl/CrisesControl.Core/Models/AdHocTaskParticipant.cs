namespace CrisesControl.Core.Models
{
    public partial class AdHocTaskParticipant
    {
        public int Id { get; set; }
        public int? ActiveIncidentTaskId { get; set; }
        public int? ParticipantTypeId { get; set; }
        public int? ParticipantUserId { get; set; }
        public int? ParticipantGroupId { get; set; }
    }
}
