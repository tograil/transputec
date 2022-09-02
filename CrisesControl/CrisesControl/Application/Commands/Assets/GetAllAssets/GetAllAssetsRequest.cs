using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAllAssets
{
    public class GetAllAssetsRequest:IRequest<GetAllAssetsResponse>
    {
        public int AssetFilter { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }
        public int Draw { get; set; }
    }
}
