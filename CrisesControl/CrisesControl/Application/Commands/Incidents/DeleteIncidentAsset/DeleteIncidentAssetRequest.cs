using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.DeleteIncidentAsset
{
    public class DeleteIncidentAssetRequest : IRequest<DeleteIncidentAssetResponse>
    {

        public int IncidentAssetId { get; set; }

        public int AssetObjMapId { get; set; }

        public int IncidentId
        {
            get; set;
        }
    }
}
