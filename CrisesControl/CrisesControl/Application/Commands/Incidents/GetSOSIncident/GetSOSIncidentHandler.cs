using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetSOSIncident
{
    public class GetSOSIncidentHandler : IRequestHandler<GetSOSIncidentRequest, GetSOSIncidentResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetSOSIncidentHandler> _logger;
        public GetSOSIncidentHandler(ILogger<GetSOSIncidentHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetSOSIncidentResponse> Handle(GetSOSIncidentRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetSOSIncident(request);
            return result;
        }
    }
}
