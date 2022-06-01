using System.Net;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan
{
    public class UpdateCompanyDRPlanResponse
    {
        public int PackageId { get; set; }
        public int CompanyId { get; set; }
        public string Message { get; set; }
    }
}
