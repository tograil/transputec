using CrisesControl.Api.Application.Commands.MediaAssets.GetAsset;
using CrisesControl.Api.Application.Commands.MediaAssets.GetAssets;

namespace CrisesControl.Api.Application.Query
{
    public interface IAssetQuery
    {
        public Task<GetAssetsResponse> GetAssets(GetAssetsRequest request);
        public Task<GetAssetResponse> GetAsset(GetAssetRequest request);

    }
}
