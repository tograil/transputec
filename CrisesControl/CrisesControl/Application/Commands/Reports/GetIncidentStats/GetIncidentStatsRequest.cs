using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentStats
{
    public class GetIncidentStatsRequest:IRequest<GetIncidentStatsResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
