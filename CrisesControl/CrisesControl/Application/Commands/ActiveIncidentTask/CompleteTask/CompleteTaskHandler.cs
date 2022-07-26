using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask
{
    public class CompleteTaskHandler : IRequestHandler<CompleteTaskRequest, CompleteTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<CompleteTaskHandler> _logger;
       
        public CompleteTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<CompleteTaskHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
            
        }
        public async Task<CompleteTaskResponse> Handle(CompleteTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.CompleteTask(request);
            return result;
        }
    }
}
