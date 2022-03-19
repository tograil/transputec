using CrisesControl.Api.Application.Commands.MediaAssets.GetAsset;

namespace CrisesControl.Api.Application.Commands.MediaAssets.GetAssets
{
    public class GetAssetsResponse
    {
        public List<GetAssetResponse> Data { get; set; }
    }
}
