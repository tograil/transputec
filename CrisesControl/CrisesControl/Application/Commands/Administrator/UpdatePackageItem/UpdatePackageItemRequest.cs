using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdatePackageItem
{
    public class UpdatePackageItemRequest:IRequest<UpdatePackageItemResponse>
    {
        public int PackageItemId { get; set; }
        public string ItemValue { get; set; }

       
    }
}
