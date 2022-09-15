using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.Login
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            //RuleFor(x => x.EmailId)
            //  .MaximumLength(150);
            //RuleFor(x => x.Password)
            //    .MaximumLength(50);
            //RuleFor(x => x.TOKEN)
            //    .MaximumLength(50);
            RuleFor(x => x.Language)
                .MaximumLength(3)
                .NotNull();
            RuleFor(x => x.DeviceType)
                .MaximumLength(50)
                .NotNull();
            //RuleFor(x => x.AppVersion)
            //    .MaximumLength(10)
            //    .NotNull();
            RuleFor(x => x.IPAddress)
                .MaximumLength(50)
                .NotNull();
           
            //RuleFor(x => x.ExtraInfo)
            //    .MaximumLength(250);
            //RuleFor(x => x.Latitude)
            //    .NotNull();
            //RuleFor(x => x.Longitude)
            //    .NotNull();
        }
    }
}
