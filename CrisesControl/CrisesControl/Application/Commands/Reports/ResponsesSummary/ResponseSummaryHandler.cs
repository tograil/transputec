using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.ResponsesSummary
{
    public class ResponseSummaryHandler :IRequestHandler<ResponseSummaryRequest, ResponseSummaryResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly ResponseSummaryValidator _responseSummaryValidator;
        public ResponseSummaryHandler(ResponseSummaryValidator responseSummaryValidator, IReportsQuery reportsQuery)
        {
            this._reportsQuery = reportsQuery;
            this._responseSummaryValidator=responseSummaryValidator;
        }

        public async Task<ResponseSummaryResponse> Handle(ResponseSummaryRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(ResponseSummaryRequest));
            await _responseSummaryValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.ResponseSummary(request);
            return result;
        }
    }
}
