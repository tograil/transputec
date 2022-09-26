using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.DeleteAssetLink
{
    public class DeleteAssetLinkHandler : IRequestHandler<DeleteAssetLinkRequest, DeleteAssetLinkResponse>
    {
        private readonly DeleteAssetLinkValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private ILogger<DeleteAssetLinkHandler> _logger;
        public DeleteAssetLinkHandler(DeleteAssetLinkValidator assetValidator, IAssetQuery assetQuery, ILogger<DeleteAssetLinkHandler> logger)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetQuery;
            _logger = logger;
        }
        public async Task<DeleteAssetLinkResponse> Handle(DeleteAssetLinkRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteAssetLinkRequest));
            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _assetQuery.DeleteAssetLink(request);
            return result;
        }
    }
}
