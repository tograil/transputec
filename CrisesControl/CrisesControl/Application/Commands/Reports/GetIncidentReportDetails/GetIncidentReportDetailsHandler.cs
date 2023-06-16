using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentReportDetails
{
    public class GetIncidentReportDetailsHandler : IRequestHandler<GetIncidentReportDetailsRequest, GetIncidentReportDetailsResponse>
    {
        private readonly IReportsQuery _reportsQuery;
        private readonly GetIncidentReportDetailsValidator _getIndidentReportDetailsValidator;
        public GetIncidentReportDetailsHandler(IReportsQuery reportsQuery, GetIncidentReportDetailsValidator getIndidentReportDetailsValidator)
        {
            this._getIndidentReportDetailsValidator = getIndidentReportDetailsValidator;
            this._reportsQuery = reportsQuery;
        }
        public async Task<GetIncidentReportDetailsResponse> Handle(GetIncidentReportDetailsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetIncidentReportDetailsRequest));
            await _getIndidentReportDetailsValidator.ValidateAndThrowAsync(request, cancellationToken);

            var result = await _reportsQuery.GetIncidentReportDetails(request);
            return result;
        }
    }
}
