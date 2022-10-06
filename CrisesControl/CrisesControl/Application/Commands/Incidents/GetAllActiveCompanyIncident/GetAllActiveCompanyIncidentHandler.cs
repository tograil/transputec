using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAllActiveCompanyIncident
{
    public class GetAllActiveCompanyIncidentHandler : IRequestHandler<GetAllActiveCompanyIncidentRequest, GetAllActiveCompanyIncidentResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetAllActiveCompanyIncidentHandler> _logger;

        public GetAllActiveCompanyIncidentHandler(ILogger<GetAllActiveCompanyIncidentHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetAllActiveCompanyIncidentResponse> Handle(GetAllActiveCompanyIncidentRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetAllActiveCompanyIncident(request);
            return result;
        }
    }
}
