using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetGroupPingReportChart
{
    public class GetGroupPingReportChartHandler : IRequestHandler<GetGroupPingReportChartRequest, GetGroupPingReportChartResponse>
    {
        private readonly ILogger<GetGroupPingReportChartHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public GetGroupPingReportChartHandler(ILogger<GetGroupPingReportChartHandler> logger, IReportsQuery reportsQuery)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetGroupPingReportChartResponse> Handle(GetGroupPingReportChartRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetGroupPingReportChart(request);
            return result;
        }
    }
}
