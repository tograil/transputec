using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentSOSRequest
{
    public class GetIncidentSOSRequestHandler : IRequestHandler<GetIncidentSOSRequest, GetIncidentSOSRequestResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentSOSRequestHandler> _logger;
        public GetIncidentSOSRequestHandler(ILogger<GetIncidentSOSRequestHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetIncidentSOSRequestResponse> Handle(GetIncidentSOSRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetIncidentSOSRequest(request);
            return result;
        }
    }
}
