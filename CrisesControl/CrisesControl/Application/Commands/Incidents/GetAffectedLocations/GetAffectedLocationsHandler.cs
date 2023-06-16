using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAffectedLocations
{
    public class GetAffectedLocationsHandler : IRequestHandler<GetAffectedLocationsRequest, GetAffectedLocationsResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetAffectedLocationsHandler> _logger;

        public GetAffectedLocationsHandler(ILogger<GetAffectedLocationsHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetAffectedLocationsResponse> Handle(GetAffectedLocationsRequest request, CancellationToken cancellationToken)
        {
            var result =await _incidentQuery.GetAffectedLocations(request);
            return result;
        }
    }
}
