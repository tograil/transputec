using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssetLink
{
    public class GetAssetLinkRequest:IRequest<GetAssetLinkResponse>
    {
        public int AssestID { get; set; }
    }
}
