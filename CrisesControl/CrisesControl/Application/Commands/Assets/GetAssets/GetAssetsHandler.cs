using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssets
{
    public class GetAssetsHandler : IRequestHandler<GetAssetsRequest, GetAssetsResponse>
    {
        private readonly GetAssetsValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private ILogger<GetAssetsHandler> _logger;
        public GetAssetsHandler(GetAssetsValidator assetValidator, IAssetQuery assetQuery, ILogger<GetAssetsHandler> logger)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetQuery;
            _logger = logger;
        }
        public async Task<GetAssetsResponse> Handle(GetAssetsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAssetsRequest));
            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _assetQuery.GetAssets(request, cancellationToken);
            return result;
        }
    }
}
