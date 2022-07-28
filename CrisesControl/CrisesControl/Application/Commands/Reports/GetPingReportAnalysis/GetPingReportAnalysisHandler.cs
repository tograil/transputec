using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Reports.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetPingReportAnalysis
{
    public class GetPingReportAnalysisHandler : IRequestHandler<GetPingReportAnalysisRequest, GetPingReportAnalysisResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly GetPingReportAnalysisValidator _getPingReportAnalysisValidator;
        private readonly ILogger<GetPingReportAnalysisHandler> _logger;
        private readonly ICurrentUser _currentUser;
        public GetPingReportAnalysisHandler(IReportsQuery reportsQuery, GetPingReportAnalysisValidator getPingReportChartValidator, ILogger<GetPingReportAnalysisHandler> logger, ICurrentUser currentUser)
        {
            this._reportsQuery = reportsQuery;
            this._getPingReportAnalysisValidator = getPingReportChartValidator;
            this._logger = logger;
            this._currentUser = currentUser;
        }
        public async Task<GetPingReportAnalysisResponse> Handle(GetPingReportAnalysisRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetPingReportAnalysisRequest));
            await _getPingReportAnalysisValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _reportsQuery.PingReportAnalysis(request);
            return result;
        }
    }
}
