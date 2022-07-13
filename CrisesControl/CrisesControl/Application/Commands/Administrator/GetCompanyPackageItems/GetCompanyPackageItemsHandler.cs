using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageItems
{
    public class GetCompanyPackageItemsHandler : IRequestHandler<GetCompanyPackageItemsRequest, GetCompanyPackageItemsResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetCompanyPackageItemsHandler> _logger;
        private readonly GetCompanyPackageItemsValidator _getCompanyPackageItemsValidator;
        public GetCompanyPackageItemsHandler(IAdminQuery adminQuery, GetCompanyPackageItemsValidator getCompanyPackageItemsValidator, ILogger<GetCompanyPackageItemsHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            this._getCompanyPackageItemsValidator = getCompanyPackageItemsValidator;
        }
        public async Task<GetCompanyPackageItemsResponse> Handle(GetCompanyPackageItemsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetCompanyPackageItemsRequest));

            await _getCompanyPackageItemsValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _adminQuery.GetCompanyPackageItems(request);
            return result;
        }
    }
}
