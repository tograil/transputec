using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.DeleteAsset
{
    public class DeleteAssetRequest:IRequest<DeleteAssetResponse>
    {
        public int AssetId { get; set; }
    }
}
