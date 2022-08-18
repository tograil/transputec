using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentAsset
{
    public class GetIncidentAssetRequest:IRequest<GetIncidentAssetResponse>
    {
        public int IncidentId { get; set; }
    }
}
