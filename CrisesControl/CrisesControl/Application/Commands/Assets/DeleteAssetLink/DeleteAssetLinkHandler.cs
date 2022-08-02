using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.DeleteAssetLink
{
    public class DeleteAssetLinkHandler : IRequestHandler<DeleteAssetLinkRequest, DeleteAssetLinkResponse>
    {
        private readonly IAssetQuery _assetQuery;
        private readonly ILogger<DeleteAssetLinkHandler> _logger;
        private readonly DeleteAssetLinkValidator _deleteAssetLinkValidator;
        public DeleteAssetLinkHandler(IAssetQuery assetQuery, ILogger<DeleteAssetLinkHandler> logger, DeleteAssetLinkValidator deleteAssetLinkValidator)
        {
            this._assetQuery = assetQuery;
            this._logger = logger;
            this._deleteAssetLinkValidator = deleteAssetLinkValidator;
        }
        public async Task<DeleteAssetLinkResponse> Handle(DeleteAssetLinkRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(DeleteAssetLinkRequest));
            await _deleteAssetLinkValidator.ValidateAsync(request, cancellationToken);
            var result = await _assetQuery.DeleteAssetLink(request);
            return result;
        }
    }
}
