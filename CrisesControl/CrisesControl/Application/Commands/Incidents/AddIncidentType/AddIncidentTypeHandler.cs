using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddIncidentType
{
    public class AddIncidentTypeHandler : IRequestHandler<AddIncidentTypeRequest, AddIncidentTypeResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<AddIncidentTypeHandler> _logger;
        public AddIncidentTypeHandler(IIncidentQuery incidentQuery, ILogger<AddIncidentTypeHandler> logger)
        {
            this._incidentQuery = incidentQuery;
            this._logger = logger;
        }
        public async Task<AddIncidentTypeResponse> Handle(AddIncidentTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.AddIncidentType(request);
            return result;
        }
    }
}
