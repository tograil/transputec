using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.MediaAssets.GetAsset
{
    public class GetAssetHandler : IRequestHandler<GetAssetRequest, GetAssetResponse>
    {
        private readonly GetAssetValidator _assetValidator;
        private readonly IAssetQuery _assetQuery; 
        public GetAssetHandler(GetAssetValidator assetValidator, IAssetQuery assetQuery)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetQuery;
        }
        public async Task<GetAssetResponse> Handle(GetAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAssetRequest));

            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);

            var asset = await _assetQuery.GetAsset(request);

            return asset;
        }
    }
}
