using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.DeleteCompanyComplete
{
    public class DeleteCompanyCompleteValidator:AbstractValidator<DeleteCompanyCompleteRequest>
    {
        public DeleteCompanyCompleteValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => string.IsNullOrEmpty(x.GUID));
        }
    }
}
