using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetUserIncidentReport
{
    public class GetUserIncidentReportHandler : IRequestHandler<GetUserIncidentReportRequest, GetUserIncidentReportResponse>
    {
        private readonly ILogger<GetUserIncidentReportHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public GetUserIncidentReportHandler(ILogger<GetUserIncidentReportHandler> logger, IReportsQuery reportsQuery)
        {
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetUserIncidentReportResponse> Handle(GetUserIncidentReportRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportsQuery.GetUserIncidentReport(request);
            return result;
        }
    }
}
