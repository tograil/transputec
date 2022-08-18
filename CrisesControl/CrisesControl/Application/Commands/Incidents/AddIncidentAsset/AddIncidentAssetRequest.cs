using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddIncidentAsset
{
    public class AddIncidentAssetRequest:IRequest<AddIncidentAssetResponse>
    {
        public int IncidentId { get; set; }
        public string LinkedAssetId { get; set; }
    }
}
