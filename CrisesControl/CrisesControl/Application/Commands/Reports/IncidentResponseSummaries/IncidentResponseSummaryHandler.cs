using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Commands.Reports.IncidentResponseSummaries;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.IncidentResponseSummary
{
    public class IncidentResponseSummaryHandler : IRequestHandler<IncidentResponseSummaryRequest, IncidentResponseSummaryResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly ILogger<IncidentResponseSummaryHandler> _logger;
        private readonly IncidentResponseSummaryValidator _incidentResponseSummaryValidator;
        public IncidentResponseSummaryHandler(IReportsQuery reportsQuery, IncidentResponseSummaryValidator incidentResponseSummaryValidator, ILogger<IncidentResponseSummaryHandler> logger)
        {
            this._incidentResponseSummaryValidator = incidentResponseSummaryValidator;
            this._reportsQuery = reportsQuery;
            this._logger = logger;
        }
        public async Task<IncidentResponseSummaryResponse> Handle(IncidentResponseSummaryRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(IncidentResponseSummaryRequest));
            await _incidentResponseSummaryValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.IncidentResponseSummary(request);
            return result;
        }
    }
}
