using MediatR;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan
{
    public class UpdateCompanyDRPlanRequest:IRequest<UpdateCompanyDRPlanResponse>
    {
        public UpdateCompanyDRPlanRequest()
        {
            IsDefault = false;
        }
       
        public int CompanyId { get; set; }

        //For DR PLan
        public string PlanName { get; set; } = null!;
        public string? PlanDescription { get; set; }
        public bool IsDefault { get; set; }=false;
        public int Status { get; set; }
        public decimal PackagePrice { get; set; }
        public bool PingOnly { get; set; }
    }
}
