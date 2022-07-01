using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.ExportAcknowledgement
{
    public class ExportAcknowledgementHandler:IRequestHandler<ExportAcknowledgementRequest, ExportAcknowledgementResponse>
    {
        private readonly ExportAcknowledgementValidator _exportAcknowledgementValidator;
        private readonly ILogger<ExportAcknowledgementHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public ExportAcknowledgementHandler(IReportsQuery reportsQuery, ILogger<ExportAcknowledgementHandler> logger, ExportAcknowledgementValidator exportAcknowledgementValidator)
        {
            this._exportAcknowledgementValidator = exportAcknowledgementValidator;
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }

        public async Task<ExportAcknowledgementResponse> Handle(ExportAcknowledgementRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ExportAcknowledgementRequest));
            await _exportAcknowledgementValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.ExportAcknowledgement(request);
            return result;
        }
    }
}
