using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskDetails
{
    public class GetTaskDetailsRequest:IRequest<GetTaskDetailsResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
    }
}
