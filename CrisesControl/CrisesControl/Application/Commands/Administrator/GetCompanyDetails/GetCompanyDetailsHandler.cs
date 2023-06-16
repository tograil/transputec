using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyDetails
{
    public class GetCompanyDetailsHandler : IRequestHandler<GetCompanyDetailsRequest, GetCompanyDetailsResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetCompanyDetailsHandler> _logger;
        private readonly GetCompanyDetailsValidator _getCompanyDetailsValidator;
        public GetCompanyDetailsHandler(IAdminQuery adminQuery, GetCompanyDetailsValidator getCompanyDetailsValidator)
        {
            this._adminQuery = adminQuery;
            this._getCompanyDetailsValidator = getCompanyDetailsValidator;
        }
        public async Task<GetCompanyDetailsResponse> Handle(GetCompanyDetailsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCompanyDetailsRequest));
            await _getCompanyDetailsValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.GetCompanyDetails(request);
            return result;
        }
    }
}
