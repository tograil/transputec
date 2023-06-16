using CrisesControl.Core.Tasks;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskAsset
{
    public class GetActiveTaskAssetResponse
    {
        public List<TaskAssetList> Data { get; set; }
        public string Message{ get; set; }
    }
}
