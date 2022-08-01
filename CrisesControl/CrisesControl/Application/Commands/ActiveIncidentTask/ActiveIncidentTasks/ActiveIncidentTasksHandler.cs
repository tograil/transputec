using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks
{
    public class ActiveIncidentTasksHandler : IRequestHandler<ActiveIncidentTasksRequest, ActiveIncidentTasksResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<ActiveIncidentTasksHandler> _logger;
        public ActiveIncidentTasksHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<ActiveIncidentTasksHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
        }
        public async Task<ActiveIncidentTasksResponse> Handle(ActiveIncidentTasksRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.ActiveIncidentTasks(request);
            return result;
        }
    }
}
