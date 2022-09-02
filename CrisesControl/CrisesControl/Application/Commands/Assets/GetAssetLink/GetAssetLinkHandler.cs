using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssetLink
{
    public class GetAssetLinkHandler : IRequestHandler<GetAssetLinkRequest, GetAssetLinkResponse>
    {
        private readonly GetAssetLinkValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private ILogger<GetAssetLinkHandler> _logger;
        public GetAssetLinkHandler(GetAssetLinkValidator assetValidator, IAssetQuery assetQuery, ILogger<GetAssetLinkHandler> logger)
        {
            _assetValidator = assetValidator;
            _assetQuery = assetQuery;
            _logger = logger;
        }
        public async Task<GetAssetLinkResponse> Handle(GetAssetLinkRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAssetLinkRequest));
            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);
            var result = await _assetQuery.GetAssetLink(request);
            return result;
        }
    }
}
