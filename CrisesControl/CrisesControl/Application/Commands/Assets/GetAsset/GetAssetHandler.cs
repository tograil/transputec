using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Assets.Respositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAsset
{
    public class GetAssetHandler : IRequestHandler<GetAssetRequest, GetAssetResponse>
    {
        private readonly GetAssetValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private ILogger<GetAssetHandler> _logger;
        public GetAssetHandler(GetAssetValidator assetValidator, IAssetQuery assetQuery, ILogger<GetAssetHandler> logger)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetQuery;
            _logger = logger;
        }
        public async Task<GetAssetResponse> Handle(GetAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAssetRequest));
            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _assetQuery.GetAsset(request, cancellationToken);
            return result;
        }
    }
}
