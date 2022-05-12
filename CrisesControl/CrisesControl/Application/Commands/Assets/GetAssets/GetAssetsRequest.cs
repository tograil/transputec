using MediatR;

namespace CrisesControl.Api.Application.Commands.Assets.GetAssets
{
    public class GetAssetsRequest: IRequest<GetAssetsResponse>
    {
        public int CompanyId { get; set; }
    }
}
