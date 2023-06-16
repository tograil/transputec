using CrisesControl.Core.Compatibility;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAllAssets
{
    public class GetAllAssetsRequest:IRequest<GetAllAssetsResponse>
    {
        public int AssetFilter { get; set; }
    }
}
