using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyDetails
{
    public class GetCompanyDetailsValidator:AbstractValidator<GetCompanyDetailsRequest>
    {
        public GetCompanyDetailsValidator()
        {
            RuleFor(x => x.CompanyID).GreaterThan(0);
        }
    }
}
