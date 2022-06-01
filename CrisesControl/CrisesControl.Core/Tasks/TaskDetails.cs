using System.Collections.Generic;

namespace CrisesControl.Core.Tasks;
public class TaskDetails
{
    public string? TaskTitle { get; set; }
    public string? TaskDescription { get; set; }
    public int TaskHeaderId { get; set; }
    public int TaskSequence { get; set; }
    public double EscalationDuration { get; set; }
    public double ExpectedCompletionTime { get; set; }
    public bool HasPredecessor { get; set; }
    public int IncidentId { get; set; }
    public int IncidentTaskId { get; set; }
    public int Status { get; set; }
    public List<Predecessor>? TaskPredecessor { get; set; }
    public List<TaskUser>? ActionUsers { get; set; }
    public List<TaskGroup>? ActionGroups { get; set; }
    public List<TaskUser>? EscalationUsers { get; set; }
    public List<TaskGroup>? EscalationGroups { get; set; }
}