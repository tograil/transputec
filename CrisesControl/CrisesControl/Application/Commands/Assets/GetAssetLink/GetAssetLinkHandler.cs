using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssetLink
{
    public class GetAssetLinkHandler : IRequestHandler<GetAssetLinkRequest, GetAssetLinkResponse>
    {
        private readonly IAssetQuery _assetQuery;
        private readonly ILogger<GetAssetLinkHandler> _logger;
        private readonly GetAssetLinkValidator _getAssetLinkValidator;
        public GetAssetLinkHandler(IAssetQuery assetQuery, ILogger<GetAssetLinkHandler> logger, GetAssetLinkValidator getAssetLinkValidator)
        {
            this._assetQuery = assetQuery;
            this._logger = logger;
            this._getAssetLinkValidator = getAssetLinkValidator;
        }
        public async Task<GetAssetLinkResponse> Handle(GetAssetLinkRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAssetLinkRequest));
            await _getAssetLinkValidator.ValidateAsync(request, cancellationToken);
            var result = await _assetQuery.GetAssetLink(request);
            return result;
        }
    }
}
