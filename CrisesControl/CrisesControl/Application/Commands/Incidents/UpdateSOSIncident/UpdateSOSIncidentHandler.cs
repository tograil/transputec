using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateSOSIncident
{
    public class UpdateSOSIncidentHandler : IRequestHandler<UpdateSOSIncidentRequest, UpdateSOSIncidentResponse>
    {
        private readonly ILogger<UpdateSOSIncidentHandler> _logger;
        private readonly IIncidentQuery _incidentQuery;
        public UpdateSOSIncidentHandler(IIncidentQuery incidentQuery, ILogger<UpdateSOSIncidentHandler> logger)
        {
            this._incidentQuery = incidentQuery;
            this._logger = logger;
        }
        public async Task<UpdateSOSIncidentResponse> Handle(UpdateSOSIncidentRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.UpdateSOSIncident(request);
            return result;
        }
    }
}
