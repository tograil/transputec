using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveTaskAsset
{
    public class SaveActiveTaskAssetRequest : IRequest<SaveActiveTaskAssetResponse>
    {
        public int ActiveTaskId { get; set; }
        public int[] TaskAssets { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }
}
