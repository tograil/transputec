using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskAsset
{
    public class GetActiveTaskAssetRequest : IRequest<GetActiveTaskAssetResponse>
    {
        public int ActiveTaskId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }
}
