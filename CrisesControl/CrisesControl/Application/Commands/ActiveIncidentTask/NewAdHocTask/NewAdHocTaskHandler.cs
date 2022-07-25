using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.NewAdHocTask
{
    public class NewAdHocTaskHandler : IRequestHandler<NewAdHocTaskRequest, NewAdHocTaskResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<NewAdHocTaskHandler> _logger;
        public NewAdHocTaskHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<NewAdHocTaskHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
        }
        public async Task<NewAdHocTaskResponse> Handle(NewAdHocTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.NewAdHocTask(request);
            return result;
        }
    }
}
