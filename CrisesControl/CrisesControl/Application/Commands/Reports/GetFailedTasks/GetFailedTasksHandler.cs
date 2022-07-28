using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetFailedTasks
{
    public class GetFailedTasksHandler : IRequestHandler<GetFailedTasksRequest, GetFailedTasksResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly ILogger<GetFailedTasksHandler> _logger;
        public GetFailedTasksHandler(IReportsQuery reportsQuery, ILogger<GetFailedTasksHandler> logger)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetFailedTasksResponse> Handle(GetFailedTasksRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetFailedTasks(request);
            return result;
        }
    }
}
