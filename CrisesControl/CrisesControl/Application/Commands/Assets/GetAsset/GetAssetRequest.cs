using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAsset
{
    public class GetAssetRequest: IRequest<GetAssetResponse>
    {
        public int AssetId { get; set; }
    }
}
