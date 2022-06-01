using CrisesControl.Core.Tasks;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.AddNewIncidentTask;

public class AddNewIncidentTaskRequest : IRequest<AddNewIncidentTaskResponse>
{
    public int IncidentTaskId { get; set; }
    public int IncidentId { get; set; }
    public int TaskHeaderId { get; set; }
    public string TaskTitle { get; set; }
    public string TaskDescription { get; set; }
    public double EscalationDuration { get; set; }
    public double ExpectedCompletionTime { get; set; }
    public int[] ActionUsers { get; set; }
    public int[] ActionGroups { get; set; }
    public int[] EscalationUsers { get; set; }
    public int[] EscalationGroups { get; set; }
    public int[] TaskPredecessor { get; set; }
    public int[] TaskAssets { get; set; }
    public bool HasPredecessor { get; set; }
    //TODO: Change Status into enum
    public int Status { get; set; }
    public int ActiveIncidentId { get; set; }
    public List<CheckList> CheckListItems { get; set; }
}