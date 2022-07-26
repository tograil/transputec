using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetPerformanceReport
{
    public class GetPerformanceReportHandler : IRequestHandler<GetPerformanceReportRequest, GetPerformanceReportResponse>
    {
        public readonly IReportsQuery _reportsQuery;
        private readonly ILogger<GetPerformanceReportHandler> _logger;
        public GetPerformanceReportHandler(IReportsQuery reportsQuery, ILogger<GetPerformanceReportHandler> logger)
        {
            this._reportsQuery = reportsQuery;
            this._logger = logger;
        }
        public async Task<GetPerformanceReportResponse> Handle(GetPerformanceReportRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetPerformanceReport(request);
            return result;
        }
    }
}
