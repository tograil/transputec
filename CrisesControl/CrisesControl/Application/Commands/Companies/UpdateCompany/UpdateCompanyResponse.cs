using System.Net;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompany
{
    public class UpdateCompanyResponse
    {
        public int CompanyId { get; set; }
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode ErrorCode { get; set; }
    }
}
