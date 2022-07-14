using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.SaveDashboard
{
    public class SaveDashboardValidator : AbstractValidator<SaveDashboardRequest>
    {
        public SaveDashboardValidator()
        {
        }
    }
}
