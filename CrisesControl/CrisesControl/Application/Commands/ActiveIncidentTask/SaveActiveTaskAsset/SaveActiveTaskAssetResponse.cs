using CrisesControl.Core.Tasks;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveTaskAsset
{
    public class SaveActiveTaskAssetResponse
    {
        public List<TaskAssetList> Data { get; set; }
        public string Message { get; set; }
    }
}
