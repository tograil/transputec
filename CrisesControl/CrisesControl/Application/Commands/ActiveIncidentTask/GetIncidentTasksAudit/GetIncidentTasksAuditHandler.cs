using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetIncidentTasksAudit
{
    public class GetIncidentTasksAuditHandler : IRequestHandler<GetIncidentTasksAuditRequest, GetIncidentTasksAuditResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetIncidentTasksAuditHandler> _logger;
        //private readonly AcceptTaskValidator _acceptTaskValidator;
        public GetIncidentTasksAuditHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<GetIncidentTasksAuditHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;

        }
        public async Task<GetIncidentTasksAuditResponse> Handle(GetIncidentTasksAuditRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.GetIncidentTasksAudit(request);
            return result;
        }
    }
}
