using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport
{
    public class GetMessageDeliveryReportHandler : IRequestHandler<GetMessageDeliveryReportRequest, GetMessageDeliveryReportResponse>
    {

        private readonly IReportsQuery _reportQuery;
        private readonly ILogger<GetMessageDeliveryReportHandler> _logger;
        private readonly GetMessageDeliveryReportValidator _getMessageDeliveryReportValidator;
        public GetMessageDeliveryReportHandler(ILogger<GetMessageDeliveryReportHandler> logger, IReportsQuery reportQuery, GetMessageDeliveryReportValidator getMessageDeliveryReportValidator)
        {
            this._reportQuery = reportQuery;
            this._logger = logger;
            this._getMessageDeliveryReportValidator= getMessageDeliveryReportValidator;
        }
        public async Task<GetMessageDeliveryReportResponse> Handle(GetMessageDeliveryReportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetMessageDeliveryReportRequest));
            await _getMessageDeliveryReportValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _reportQuery.GetMessageDeliveryReport(request);
            return result;
        }
    }
}
