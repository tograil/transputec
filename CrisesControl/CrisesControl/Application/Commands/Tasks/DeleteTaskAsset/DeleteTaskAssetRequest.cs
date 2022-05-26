using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.DeleteTaskAsset;

public class DeleteTaskAssetRequest : IRequest<List<CrisesControl.Core.Assets.Assets>>
{
    public int IncidentTaskId { get; set; }
    public int[] TaskAssets { get; set; }
    public int CurrentUserId { get; set; }
    public int CompanyId { get; set; }
}