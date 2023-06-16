using CrisesControl.Core.Incidents;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentAsset
{
    public class GetIncidentAssetResponse
    {
        public List<IncidentAssets> Data { get; set; }
        public string Message { get; set; }
    }
}
