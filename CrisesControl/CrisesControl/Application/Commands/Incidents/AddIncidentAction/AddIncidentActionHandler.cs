using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddIncidentAction
{
    public class AddIncidentActionHandler : IRequestHandler<AddIncidentActionRequest, AddIncidentActionResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<AddIncidentActionHandler> _logger;
        public AddIncidentActionHandler(IIncidentQuery incidentQuery, ILogger<AddIncidentActionHandler> logger)
        {
            this._incidentQuery = incidentQuery;
            this._logger = logger;
        }
        public async Task<AddIncidentActionResponse> Handle(AddIncidentActionRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.AddIncidentAction(request);
            return result;
        }
    }
}
