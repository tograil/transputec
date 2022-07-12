using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetReport
{
    public class GetReportHandler : IRequestHandler<GetReportRequest, GetReportResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly GetReportValidator _getReportValidator;
        public GetReportHandler(GetReportValidator getReportValidator, IAdminQuery adminQuery)
        {
            this._adminQuery = adminQuery;
            this._getReportValidator = getReportValidator;
        }
        public async Task<GetReportResponse> Handle(GetReportRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetReportRequest));

            await _getReportValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.GetReport(request);
            return result;
        }
    }
}
