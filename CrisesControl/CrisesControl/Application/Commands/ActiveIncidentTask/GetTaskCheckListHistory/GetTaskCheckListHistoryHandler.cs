using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskCheckListHistory
{
    public class GetTaskCheckListHistoryHandler : IRequestHandler<GetTaskCheckListHistoryRequest, GetTaskCheckListHistoryResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetTaskCheckListHistoryHandler> _logger;
        public GetTaskCheckListHistoryHandler(IActiveIncidentQuery activeIncidentQuery,ILogger<GetTaskCheckListHistoryHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
        }
        public async Task<GetTaskCheckListHistoryResponse> Handle(GetTaskCheckListHistoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.GetTaskCheckListHistory(request);
            return result;
        }
    }
}
