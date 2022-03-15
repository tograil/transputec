using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Groups.UpdateGroup
{
    public class UpdateGroupValidator : AbstractValidator<UpdateGroupRequest>
    {
        public UpdateGroupValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
