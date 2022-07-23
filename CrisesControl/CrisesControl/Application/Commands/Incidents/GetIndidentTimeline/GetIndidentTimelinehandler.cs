using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIndidentTimeline
{
    public class GetIndidentTimelinehandler : IRequestHandler<GetIndidentTimelineRequest, GetIndidentTimelineResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIndidentTimelinehandler> _logger;
        public GetIndidentTimelinehandler(ILogger<GetIndidentTimelinehandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetIndidentTimelineResponse> Handle(GetIndidentTimelineRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetIndidentTimeline(request);
            return result;
        }
    }
}
