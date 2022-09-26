using CrisesControl.Api.Application.Commands.Assets.CreateAsset;
using CrisesControl.Api.Application.Commands.Assets.DeleteAsset;
using CrisesControl.Api.Application.Commands.Assets.DeleteAssetLink;
using CrisesControl.Api.Application.Commands.Assets.GetAllAssets;
using CrisesControl.Api.Application.Commands.Assets.GetAsset;
using CrisesControl.Api.Application.Commands.Assets.GetAssetLink;
using CrisesControl.Api.Application.Commands.Assets.GetAssets;
using CrisesControl.Api.Application.Commands.Assets.UpdateAssets;

namespace CrisesControl.Api.Application.Query
{
    public interface IAssetQuery
    {
        public Task<GetAssetsResponse> GetAssets(GetAssetsRequest request, CancellationToken cancellationToken);
        Task<GetAllAssetsResponse> GetAllAssets(GetAllAssetsRequest request);
        public Task<GetAssetResponse> GetAsset(GetAssetRequest request, CancellationToken cancellationToken);
        Task<DeleteAssetResponse> DeleteAsset(DeleteAssetRequest request, CancellationToken cancellationToken);
        Task<GetAssetLinkResponse> GetAssetLink(GetAssetLinkRequest request);
        Task<DeleteAssetLinkResponse> DeleteAssetLink(DeleteAssetLinkRequest request);
        Task<CreateAssetResponse> CreateAsset(CreateAssetRequest request, CancellationToken cancellationToken);
        Task<UpdateAssetsResponse> UpdateAssets(UpdateAssetsRequest updateAssetsRequest, CancellationToken cancellationToken);

    }
}
