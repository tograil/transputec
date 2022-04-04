using AutoMapper;
using CrisesControl.Api.Application.Commands.MediaAssets.GetAsset;
using CrisesControl.Api.Application.Commands.MediaAssets.GetAssets;
using CrisesControl.Core.AssetAggregate;
using CrisesControl.Core.AssetAggregate.Respositories;

namespace CrisesControl.Api.Application.Query
{
    public class AssetQuery: IAssetQuery
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IMapper _mapper;

        public AssetQuery(IAssetRepository assetRepository, IMapper mapper)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
        }

        public async Task<GetAssetsResponse> GetAssets(GetAssetsRequest request)
        {
            var assets = await _assetRepository.GetAllAssets(request.CompanyId);
            List<GetAssetResponse> response = _mapper.Map<List<Asset>, List<GetAssetResponse>>(assets.ToList());
            var result = new GetAssetsResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetAssetResponse> GetAsset(GetAssetRequest request)
        {
            var asset = await _assetRepository.GetAsset(request.CompanyId, request.AssetId);
            GetAssetResponse response = _mapper.Map<Asset, GetAssetResponse>(asset);

            return response;
        }
    }
}
