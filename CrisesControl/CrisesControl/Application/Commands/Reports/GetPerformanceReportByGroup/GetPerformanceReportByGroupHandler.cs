using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetPerformanceReportByGroup
{
    public class GetPerformanceReportByGroupHandler : IRequestHandler<GetPerformanceReportByGroupRequest, GetPerformanceReportByGroupResponse>
    {
        public readonly IReportsQuery _reportsQuery;
        private readonly ILogger<GetPerformanceReportByGroupHandler> _logger;
        public GetPerformanceReportByGroupHandler(IReportsQuery reportsQuery, ILogger<GetPerformanceReportByGroupHandler> logger)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetPerformanceReportByGroupResponse> Handle(GetPerformanceReportByGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetPerformanceReportByGroup(request);
            return result;
        }
    }
}
