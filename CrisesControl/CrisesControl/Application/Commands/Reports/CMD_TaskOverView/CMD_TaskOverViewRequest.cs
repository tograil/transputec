using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.CMD_TaskOverView
{
    public class CMD_TaskOverViewRequest : IRequest<CMD_TaskOverViewResponse>
    {
        public int IncidentActivationId { get; set; }
        public int SelectedUserId { get; set; }
    }
}
