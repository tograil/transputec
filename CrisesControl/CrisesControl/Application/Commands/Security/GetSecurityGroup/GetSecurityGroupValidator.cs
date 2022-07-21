using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Security.GetSecurityGroup
{
    public class GetSecurityGroupValidator:AbstractValidator<GetSecurityGroupRequest>
    {
        public GetSecurityGroupValidator()
        {
            RuleFor(x=>x.SecurityGroupId).GreaterThan(0);
        }
    }
}
