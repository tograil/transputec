using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAllAssets
{
    public class GetAllAssetsHandler : IRequestHandler<GetAllAssetsRequest, GetAllAssetsResponse>
    {
        private readonly GetAllAssetsValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private ILogger<GetAllAssetsHandler> _logger;
        public GetAllAssetsHandler(GetAllAssetsValidator assetValidator, IAssetQuery assetQuery, ILogger<GetAllAssetsHandler> logger)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetQuery;
            _logger = logger;
        }
        public async Task<GetAllAssetsResponse> Handle(GetAllAssetsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAllAssetsRequest));
            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _assetQuery.GetAllAssets(request);
            return result;
        }
    }
}
