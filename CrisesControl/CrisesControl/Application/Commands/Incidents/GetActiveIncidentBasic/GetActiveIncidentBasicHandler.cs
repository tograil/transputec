using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetActiveIncidentBasic
{
    public class GetActiveIncidentBasicHandler : IRequestHandler<GetActiveIncidentBasicRequest, GetActiveIncidentBasicResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetActiveIncidentBasicHandler> _logger;
        public GetActiveIncidentBasicHandler(ILogger<GetActiveIncidentBasicHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetActiveIncidentBasicResponse> Handle(GetActiveIncidentBasicRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetActiveIncidentBasic(request);
            return result;
        }
    }
}
