using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Groups.CheckGroup
{
    public class CheckGroupValidator:AbstractValidator<CheckGroupRequest>
    {
        public CheckGroupValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.GroupName));
            RuleFor(x => x.CompanyId).GreaterThan(0);
        }
    }
}
