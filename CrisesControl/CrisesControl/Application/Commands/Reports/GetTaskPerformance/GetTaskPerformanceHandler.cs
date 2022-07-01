using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetTaskPerformance
{
    public class GetTaskPerformanceHandler : IRequestHandler<GetTaskPerformanceRequest, GetTaskPerformanceResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly ILogger<GetTaskPerformanceHandler> _logger;
        public GetTaskPerformanceHandler(IReportsQuery reportsQuery, ILogger<GetTaskPerformanceHandler> logger)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetTaskPerformanceResponse> Handle(GetTaskPerformanceRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetTaskPerformance(request);
            return result;
        }
    }
}
