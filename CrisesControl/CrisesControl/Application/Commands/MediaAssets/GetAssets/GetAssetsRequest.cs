using MediatR;

namespace CrisesControl.Api.Application.Commands.MediaAssets.GetAssets
{
    public class GetAssetsRequest: IRequest<GetAssetsResponse>
    {
        public int CompanyId { get; set; }
    }
}
