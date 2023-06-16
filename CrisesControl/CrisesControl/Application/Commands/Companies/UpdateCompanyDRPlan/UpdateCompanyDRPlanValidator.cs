using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.UpdateCompanyDRPlan
{
    public class UpdateCompanyDRPlanValidator: AbstractValidator<UpdateCompanyDRPlanRequest>
    {
        public UpdateCompanyDRPlanValidator()
        {
            RuleFor(x => x.CompanyId)
              .GreaterThan(0);
            RuleFor(x=>string.IsNullOrEmpty(x.DRPlan));
              
        }
      
    }
}
