using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.DownloadDeliveryReport
{
    public class DownloadDeliveryReportHandler : IRequestHandler<DownloadDeliveryReportRequest, DownloadDeliveryReportResponse>
    {
        private readonly DownloadDeliveryReportValidator _downloadDeliveryReportValidator;
        private readonly ILogger<DownloadDeliveryReportHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public DownloadDeliveryReportHandler(IReportsQuery reportsQuery, ILogger<DownloadDeliveryReportHandler> logger, DownloadDeliveryReportValidator downloadDeliveryReportValidator)
        {
            this._downloadDeliveryReportValidator = downloadDeliveryReportValidator;
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<DownloadDeliveryReportResponse> Handle(DownloadDeliveryReportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DownloadDeliveryReportRequest));
            await _downloadDeliveryReportValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.DownloadDeliveryReport(request);
            return result;
        }
    }
}
