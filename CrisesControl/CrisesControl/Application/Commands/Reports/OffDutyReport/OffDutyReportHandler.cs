using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.OffDutyReport
{
    public class OffDutyReportHandler : IRequestHandler<OffDutyReportRequest, OffDutyReportResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly OffDutyReportValidator _offDutyReportValidator;
        private readonly ILogger<OffDutyReportHandler> _logger;
        public OffDutyReportHandler(IReportsQuery reportsQuery, OffDutyReportValidator offDutyReportValidator, ILogger<OffDutyReportHandler> logger)
        {
            this._logger = logger;
            this._offDutyReportValidator = offDutyReportValidator;
            this._reportsQuery = reportsQuery;
        }
        public async Task<OffDutyReportResponse> Handle(OffDutyReportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(OffDutyReportRequest));
            await _offDutyReportValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.OffDutyReport(request);
            return result;
        }
    }
}
