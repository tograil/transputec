using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.AddDashlet
{
    public class AddDashletValidator : AbstractValidator<AddDashletRequest>
    {
        public AddDashletValidator()
        {
            RuleFor(x => x.ModuleId)
                .GreaterThan(0);
            RuleFor(x => x.UserId)
                .GreaterThan(0);
            RuleFor(x => x.XPos)
                .GreaterThan(0);
            RuleFor(x => x.YPos)
                .GreaterThan(0);
        }
    }
}
