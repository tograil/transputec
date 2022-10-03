using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentTimeline
{
    public class GetIncidentTimelinehandler : IRequestHandler<GetIncidentTimelineRequest, GetIncidentTimelineResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentTimelinehandler> _logger;
        public GetIncidentTimelinehandler(ILogger<GetIncidentTimelinehandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetIncidentTimelineResponse> Handle(GetIncidentTimelineRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetIncidentTimeline(request);
            return result;
        }
    }
}
