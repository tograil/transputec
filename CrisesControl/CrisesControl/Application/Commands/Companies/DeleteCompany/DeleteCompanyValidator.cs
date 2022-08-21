using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.DeleteCompany
{
    public class DeleteCompanyValidator:AbstractValidator<DeleteCompanyRequest>
    {
        public DeleteCompanyValidator()
        {
            RuleFor(x => x.TargetCompanyID).GreaterThan(0);
        }
    }
}
