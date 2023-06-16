using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.UpdateSOPAsset
{
    public class UpdateSOPAssetRequest:IRequest<UpdateSOPAssetResponse>
    {
        public int SOPHeaderID { get; set; }
        public int AssetID { get; set; }
    }
}
