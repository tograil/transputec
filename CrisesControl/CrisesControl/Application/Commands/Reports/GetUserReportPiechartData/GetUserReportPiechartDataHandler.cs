using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserReportPiechartData
{
    public class GetUserReportPiechartDataHandler:IRequestHandler<GetUserReportPiechartDataRequest, GetUserReportPiechartDataResponse>
    {
        private readonly ILogger<GetUserReportPiechartDataHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public GetUserReportPiechartDataHandler(ILogger<GetUserReportPiechartDataHandler> logger, IReportsQuery reportsQuery)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }

        public async Task<GetUserReportPiechartDataResponse> Handle(GetUserReportPiechartDataRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetUserReportPiechartData(request);
            return result;
        }
    }
}
