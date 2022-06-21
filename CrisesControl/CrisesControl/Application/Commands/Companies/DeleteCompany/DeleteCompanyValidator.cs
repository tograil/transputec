using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.DeleteCompany
{
    public class DeleteCompanyValidator:AbstractValidator<DeleteCompanyRequest>
    {
        public DeleteCompanyValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => string.IsNullOrEmpty(x.GUID));
        }
    }
}
