using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Security.DeleteSecurityGroup
{
    public class DeleteSecurityGroupValidator:AbstractValidator<DeleteSecurityGroupRequest>
    {
        public DeleteSecurityGroupValidator()
        {
            RuleFor(x => x.SecurityGroupId).GreaterThan(0);
        }
    }
}
