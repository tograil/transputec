using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyDetails
{
    public class GetCompanyDetailsResponse
    {
        public CompanyDetails Data { get; set; }
        public string Message { get; set; }
    }
}
