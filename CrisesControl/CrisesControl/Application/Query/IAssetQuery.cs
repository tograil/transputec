using CrisesControl.Api.Application.Commands.Assets.GetAsset;
using CrisesControl.Api.Application.Commands.Assets.GetAssets;

namespace CrisesControl.Api.Application.Query
{
    public interface IAssetQuery
    {
        public Task<GetAssetsResponse> GetAssets(GetAssetsRequest request, CancellationToken cancellationToken);
        public Task<GetAssetResponse> GetAsset(GetAssetRequest request, CancellationToken cancellationToken);

    }
}
