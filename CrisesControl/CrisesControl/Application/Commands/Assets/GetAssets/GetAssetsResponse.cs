using CrisesControl.Api.Application.Commands.Assets.GetAsset;
using CrisesControl.Core.Assets;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssets
{
    public class GetAssetsResponse
    {
        public List<CrisesControl.Core.Assets.Assets> Data { get; set; }
    }
}
