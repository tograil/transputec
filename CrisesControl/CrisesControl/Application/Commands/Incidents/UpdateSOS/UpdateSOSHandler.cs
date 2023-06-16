using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateSOS
{
    public class UpdateSOSHandler : IRequestHandler<UpdateSOSRequest, UpdateSOSResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<UpdateSOSHandler> _logger;
        public UpdateSOSHandler(ILogger<UpdateSOSHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<UpdateSOSResponse> Handle(UpdateSOSRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.UpdateSOS(request);
            return result;
        }
    }
}
