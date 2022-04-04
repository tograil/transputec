using MediatR;

namespace CrisesControl.Api.Application.Commands.MediaAssets.CreateAsset
{
    public class CreateAssetRequest: IRequest<CreateAssetResponse>
    {
        public int AssetId { get; set; }
        public int CompanyId { get; set; }
    }
}
