using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask
{
    public class GetUserTaskHandler : IRequestHandler<GetUserTaskRequest, GetUserTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetUserTaskHandler> _logger;
        public GetUserTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<GetUserTaskHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
        }
        public async Task<GetUserTaskResponse> Handle(GetUserTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.GetUserTasks(request);
            return result;
        }
    }
}
