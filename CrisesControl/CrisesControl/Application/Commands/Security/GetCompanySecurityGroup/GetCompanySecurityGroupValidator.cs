using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup
{
    public class GetCompanySecurityGroupValidator : AbstractValidator<GetCompanySecurityGroupRequest>
    {
        public GetCompanySecurityGroupValidator()
        {
            RuleFor(x => x.CompanyID)
                .GreaterThan(0);
        }
    }
}
