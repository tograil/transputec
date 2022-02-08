namespace CrisesControl.Core.Models
{
    public partial class TaskActionType
    {
        public int TaskActionTypeId { get; set; }
        public string? ActionTypeName { get; set; }
        public string? ActionTypeDescription { get; set; }
        public bool SendNotification { get; set; }
        public bool NoteRequired { get; set; }
        public bool UpdateTaskParticipantOnly { get; set; }
    }
}
