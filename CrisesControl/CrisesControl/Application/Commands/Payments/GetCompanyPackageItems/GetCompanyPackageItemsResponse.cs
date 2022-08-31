using CrisesControl.Core.Payments;

namespace CrisesControl.Api.Application.Commands.Payments.GetCompanyPackageItems
{
    public class GetCompanyPackageItemsResponse
    {
        public CompanyPackage Data { get; set; }
        public string Message { get; set; }
    }
}
