using AutoMapper;
using CrisesControl.Api.Application.Commands.Assets.GetAsset;
using CrisesControl.Api.Application.Commands.Assets.GetAssets;
using CrisesControl.Core.Assets;
using CrisesControl.Core.Assets.Respositories;

namespace CrisesControl.Api.Application.Query
{
    public class AssetQuery : IAssetQuery
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;

        public AssetQuery(IAssetRepository assetRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
        }

        public async Task<GetAssetsResponse> GetAssets(GetAssetsRequest request, CancellationToken cancellationToken)
        {
            var assets = await _assetRepository.GetAllAssets(request.CompanyId);
            List<GetAssetResponse> response = _mapper.Map<List<Assets>, List<GetAssetResponse>>(assets.ToList());
            var result = new GetAssetsResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetAssetResponse> GetAsset(GetAssetRequest request, CancellationToken cancellationToken)
        {
            var asset = await _assetRepository.GetAsset(request.CompanyId, request.AssetId);
            GetAssetResponse response = _mapper.Map<Assets, GetAssetResponse>(asset);

            return response;
        }
    }
}
