using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats {
    public class GetIncidentPingStatsHandler : IRequestHandler<GetIncidentPingStatsRequest, GetIncidentPingStatsResponse> {
        private readonly IReportsQuery _reportsQuery;
        private readonly GetIncidentPingStatsValidator _getIncidentPingStatsValidator;

        public GetIncidentPingStatsHandler(IReportsQuery reportsQuery, GetIncidentPingStatsValidator getMessageResponseValidator) {
            _reportsQuery = reportsQuery;
            _getIncidentPingStatsValidator = getMessageResponseValidator;
        }

        public async Task<GetIncidentPingStatsResponse> Handle(GetIncidentPingStatsRequest request, CancellationToken cancellationToken) {
            Guard.Against.Null(request, nameof(GetIncidentPingStatsRequest));
            await _getIncidentPingStatsValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetIncidentPingStats(request);
            return result;
        }
    }
}
