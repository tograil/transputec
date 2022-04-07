using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.Commands.MediaAssets.GetAsset;
using CrisesControl.Api.Application.Query;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.MediaAssets.GetAssets
{
    public class GetAssetsHandler : IRequestHandler<GetAssetsRequest, GetAssetsResponse>
    {
        private readonly GetAssetsValidator _assetValidator;
        private readonly IAssetQuery _assetQuery;
        private readonly IMapper _mapper;

        public GetAssetsHandler(GetAssetsValidator assetsValidator, IAssetQuery assetQuery, IMapper mapper)
        {
            _assetValidator = assetsValidator;
            _assetQuery = assetQuery;
            _mapper = mapper;
        }
        public async Task<GetAssetsResponse> Handle(GetAssetsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetAssetsRequest));

            await _assetValidator.ValidateAndThrowAsync(request, cancellationToken);

            var assets = await _assetQuery.GetAssets(request);

            return assets;
        }
    }
}
