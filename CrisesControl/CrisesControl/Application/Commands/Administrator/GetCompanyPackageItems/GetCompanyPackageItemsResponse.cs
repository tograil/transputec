using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageItems
{
    public class GetCompanyPackageItemsResponse
    {
        public CompanyPackageItems Data { get; set; }
        public string Message { get; set; }
    }
}
