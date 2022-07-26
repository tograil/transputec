using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentStats
{
    public class GetIncidentStatsHandler : IRequestHandler<GetIncidentStatsRequest, GetIncidentStatsResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly GetIncidentStatsValidator _getIncidentStatsValidator;
        private readonly ILogger<GetIncidentStatsHandler> _logger;
        public GetIncidentStatsHandler(IReportsQuery reportsQuery, GetIncidentStatsValidator getIncidentStatsValidator, ILogger<GetIncidentStatsHandler> logger)
        {
            this._getIncidentStatsValidator = getIncidentStatsValidator;
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetIncidentStatsResponse> Handle(GetIncidentStatsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentStatsRequest));
            await _getIncidentStatsValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetIncidentStats(request);
            return result;
        }
    }
}
