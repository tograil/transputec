using MediatR;

namespace CrisesControl.Api.Application.Commands.Support.GetIncidentStats
{
    public class GetIncidentStatsRequest : IRequest<GetIncidentStatsResponse>
    {
        public int IncidentActivationId { get; set; }
        public int OutUserCompanyId { get; set; }
    }
}
