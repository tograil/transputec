using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Groups.CreateGroup
{
    public class CreateGroupValidator : AbstractValidator<CreateGroupRequest>
    {
        public CreateGroupValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
