using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskCheckList
{
    public class GetActiveTaskCheckListHandler : IRequestHandler<GetActiveTaskCheckListRequest, GetActiveTaskCheckListResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetActiveTaskCheckListHandler> _logger;
        
        public GetActiveTaskCheckListHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<GetActiveTaskCheckListHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
           
        }
        public async Task<GetActiveTaskCheckListResponse> Handle(GetActiveTaskCheckListRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.GetActiveTaskCheckList(request);
            return result;
        }
    }
}
