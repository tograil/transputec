using System.Net;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan
{
    public class UpdateCompanyDRPlanResponse
    {
        public int PackageId { get; set; }
        public int CompanyId { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
