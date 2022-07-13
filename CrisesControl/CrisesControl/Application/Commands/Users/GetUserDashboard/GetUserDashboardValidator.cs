using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUserDashboard
{
    public class GetUserDashboardValidator : AbstractValidator<GetUserDashboardRequest>
    {
        public GetUserDashboardValidator()
        {
            RuleFor(x => x.UserID)
                .GreaterThan(0);
            RuleFor(x => x.ModuleID)
                .GreaterThan(0);
            RuleFor(x => x.ModulePage)
                .NotEmpty();
        }
    }
}
