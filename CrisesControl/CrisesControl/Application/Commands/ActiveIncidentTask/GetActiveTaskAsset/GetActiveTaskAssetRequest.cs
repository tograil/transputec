using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskAsset
{
    public class GetActiveTaskAssetRequest:IRequest<GetActiveTaskAssetResponse>
    {
        public int ActiveTaskID { get; set; }
    }
}
