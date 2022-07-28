using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.UnattendedTask
{
    public class UnattendedTaskHandler : IRequestHandler<UnattendedTaskRequest, UnattendedTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<UnattendedTaskHandler> _logger;
        public UnattendedTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<UnattendedTaskHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
        }
        public async Task<UnattendedTaskResponse> Handle(UnattendedTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.UnattendedTask(request);
            return result;
        }
    }
}
