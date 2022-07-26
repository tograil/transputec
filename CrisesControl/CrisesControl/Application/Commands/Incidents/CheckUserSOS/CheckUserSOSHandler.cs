using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.CheckUserSOS
{
    public class CheckUserSOSHandler : IRequestHandler<CheckUserSOSRequest, CheckUserSOSResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<CheckUserSOSHandler> _logger;
        public CheckUserSOSHandler(ILogger<CheckUserSOSHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<CheckUserSOSResponse> Handle(CheckUserSOSRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.CheckUserSOS(request);
            return result;
        }
    }
}
