using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageItems
{
    public class GetCompanyPackageItemsRequest:IRequest<GetCompanyPackageItemsResponse>
    {
        public int PackageItemId { get; set; }
    }
}
