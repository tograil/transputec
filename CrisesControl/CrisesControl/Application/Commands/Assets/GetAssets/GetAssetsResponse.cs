using CrisesControl.Api.Application.Commands.Assets.GetAsset;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssets
{
    public class GetAssetsResponse
    {
        public List<GetAssetResponse> Data { get; set; }
    }
}
