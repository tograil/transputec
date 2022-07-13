using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.CheckUserLicense
{
    public class CheckUserLicenseValidator : AbstractValidator<CheckUserLicenseRequest>
    {
        public CheckUserLicenseValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty();
            RuleFor(x => x.UserList)
                .NotEmpty();
        }
    }
}
