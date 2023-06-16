using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentReport
{
    public class GetIncidentReportHandler : IRequestHandler<GetIncidentReportRequest, GetIncidentReportResponse>
    {
        private readonly ILogger<GetIncidentReportHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public GetIncidentReportHandler(ILogger<GetIncidentReportHandler> logger, IReportsQuery reportsQuery)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetIncidentReportResponse> Handle(GetIncidentReportRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetIncidentReport(request);
            return result;
        }
    }
}
