using CrisesControl.Core.Tasks;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.SaveTaskCheckList;

public class SaveTaskCheckListRequest : IRequest<bool>
{
    public List<CheckList> CheckListItems { get; set; }
    public int IncidentTaskId { get; set; }
}