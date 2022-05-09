using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan
{
    public class UpdateCompanyDRPlanRequest:IRequest<UpdateCompanyDRPlanResponse>
    {     
        public int CompanyId { get; set; }
        public string DRPlan { get; set; }
    }
}
