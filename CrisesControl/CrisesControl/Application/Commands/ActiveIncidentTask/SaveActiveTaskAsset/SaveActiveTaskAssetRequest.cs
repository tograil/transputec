using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveTaskAsset
{
    public class SaveActiveTaskAssetRequest:IRequest<SaveActiveTaskAssetResponse>
    {
        public int ActiveIncidentTaskID { get; set; }       
        public int[] TaskAssets { get; set; }
       
    }
}
