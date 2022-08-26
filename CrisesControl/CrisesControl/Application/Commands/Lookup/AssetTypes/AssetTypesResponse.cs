using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Lookup.AssetTypes
{
    public class AssetTypesResponse
    {
        public List<AssetType> Data { get; set; }
        public string Message { get; set; }
    }
}
