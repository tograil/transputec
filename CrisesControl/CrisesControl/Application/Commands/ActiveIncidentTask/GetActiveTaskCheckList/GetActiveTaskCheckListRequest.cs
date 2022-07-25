using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskCheckList
{
    public class GetActiveTaskCheckListRequest:IRequest<GetActiveTaskCheckListResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
    }
}
