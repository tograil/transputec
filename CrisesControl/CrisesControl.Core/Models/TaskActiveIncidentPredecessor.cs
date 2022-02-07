namespace CrisesControl.Core.Models
{
    public partial class TaskActiveIncidentPredecessor
    {
        public int TaskPredecessorId { get; set; }
        public int ActiveIncidentTaskId { get; set; }
        public int PredecessorTaskId { get; set; }
        public string? Status { get; set; }
    }
}
