using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.DeleteAsset
{
    public class DeleteAssetHandler : IRequestHandler<DeleteAssetRequest, DeleteAssetResponse>
    {
        private readonly DeleteAssetValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private readonly ILogger<DeleteAssetHandler> _logger;

        public DeleteAssetHandler(DeleteAssetValidator assetValidator, IAssetQuery assetRepository, ILogger<DeleteAssetHandler> logger)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetRepository;
            _logger = logger;

        }
        public async Task<DeleteAssetResponse> Handle(DeleteAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteAssetRequest));
            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _assetQuery.DeleteAsset(request, cancellationToken);
            return result;
        }
    }
}
