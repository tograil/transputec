using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentByName
{
    public class GetIncidentByNameHandler : IRequestHandler<GetIncidentByNameRequest, GetIncidentByNameResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentByNameHandler> _logger;
        public GetIncidentByNameHandler(ILogger<GetIncidentByNameHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetIncidentByNameResponse> Handle(GetIncidentByNameRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetIncidentByName(request);
            return result;
        }
    }
}
