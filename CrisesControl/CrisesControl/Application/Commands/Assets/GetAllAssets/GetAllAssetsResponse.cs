using CrisesControl.Core.Compatibility;

namespace CrisesControl.Api.Application.Commands.Assets.GetAllAssets
{
    public class GetAllAssetsResponse
    {
        public DataTablePaging Data { get; set; }
        public string Message { get; set; }
    }
}
