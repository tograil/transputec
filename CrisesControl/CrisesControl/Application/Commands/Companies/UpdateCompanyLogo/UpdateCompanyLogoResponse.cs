using System.Net;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyLogo
{
    public class UpdateCompanyLogoResponse
    {
        public string CompanyLogo { get; set; }
        public int CompanyId { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
