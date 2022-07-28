using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentReportDetails
{
    public class GetIndidentReportDetailsHandler : IRequestHandler<GetIndidentReportDetailsRequest, GetIndidentReportDetailsResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly GetIndidentReportDetailsValidator _getIndidentReportDetailsValidator;
        public GetIndidentReportDetailsHandler(IReportsQuery reportsQuery, GetIndidentReportDetailsValidator getIndidentReportDetailsValidator)
        {
            this._getIndidentReportDetailsValidator = getIndidentReportDetailsValidator;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetIndidentReportDetailsResponse> Handle(GetIndidentReportDetailsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIndidentReportDetailsRequest));
            await _getIndidentReportDetailsValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetIndidentReportDetails(request);
            return result;
        }
    }
}
