using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserTracking
{
    public class GetUserTrackingRequest : IRequest<GetUserTrackingResponse>
    {
        public string Source { get; set; }
        public int UserId { get; set; }
        public int ActiveIncidentId { get; set; }
    }
}
