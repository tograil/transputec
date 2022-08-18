using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCompanySOS
{
    public class GetCompanySOSHandler : IRequestHandler<GetCompanySOSRequest, GetCompanySOSResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetCompanySOSHandler> _logger;
        public GetCompanySOSHandler(ILogger<GetCompanySOSHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetCompanySOSResponse> Handle(GetCompanySOSRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetCompanySOS(request);
            return result;
        }
    }
}
