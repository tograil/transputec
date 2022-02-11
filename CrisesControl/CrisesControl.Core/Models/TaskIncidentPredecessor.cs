namespace CrisesControl.Core.Models
{
    public partial class TaskIncidentPredecessor
    {
        public int TaskPredecessorId { get; set; }
        public int IncidentTaskId { get; set; }
        public int PredecessorTaskId { get; set; }
    }
}
