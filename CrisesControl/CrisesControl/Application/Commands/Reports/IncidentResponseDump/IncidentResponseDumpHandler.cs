using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.IncidentResponseDump
{
    public class IncidentResponseDumpHandler : IRequestHandler<IncidentResponseDumpRequest, IncidentResponseDumpResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly ILogger<IncidentResponseDumpHandler> _logger;
        private readonly IncidentResponseDumpValidator _incidentResponseDumpValidator;
        public IncidentResponseDumpHandler(IReportsQuery reportsQuery, IncidentResponseDumpValidator incidentResponseSummaryValidator, ILogger<IncidentResponseDumpHandler> logger)
        {
            this._incidentResponseDumpValidator = incidentResponseSummaryValidator;
            this._reportsQuery = reportsQuery;
            this._logger = logger;
        }
        public async Task<IncidentResponseDumpResponse> Handle(IncidentResponseDumpRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(IncidentResponseDumpRequest));
            await _incidentResponseDumpValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.IncidentResponseDump(request);
            return result;
        }
    }
}
