using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskCheckListHistory
{
    public class GetTaskCheckListHistoryRequest:IRequest<GetTaskCheckListHistoryResponse>
    {
        public int ActiveCheckListId { get; set; }
    }
}
