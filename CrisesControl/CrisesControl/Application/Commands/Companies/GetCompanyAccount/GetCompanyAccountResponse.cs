using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Commands.Companies.GetCompanyAccount
{
    public class GetCompanyAccountResponse
    {
        public CompanyAccount Account { get; set; }
        public string Message { get; set; }
    }
}
