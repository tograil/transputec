using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetFailedAttempts
{
    public class GetFailedAttemptsHandler : IRequestHandler<GetFailedAttemptsRequest, GetFailedAttemptsResponse>
    {
        private readonly GetFailedAttemptsValidator _getFailedAttemptsValidator;
        private readonly ILogger<GetFailedAttemptsHandler> _logger;
        private readonly IReportsQuery _reportsQuery;
        public GetFailedAttemptsHandler(IReportsQuery reportsQuery, ILogger<GetFailedAttemptsHandler> logger, GetFailedAttemptsValidator getFailedAttemptsValidator)
        {
            this._getFailedAttemptsValidator = getFailedAttemptsValidator;
            this._logger = logger;
            this._reportsQuery = reportsQuery;
        }

        public async Task<GetFailedAttemptsResponse> Handle(GetFailedAttemptsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetFailedAttemptsRequest));
            await _getFailedAttemptsValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetFailedAttempts(request);
            return result;
        }
    }
}
