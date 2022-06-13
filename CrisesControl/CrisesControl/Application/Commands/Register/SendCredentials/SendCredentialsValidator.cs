using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.SendCredentials
{
    public class SendCredentialsValidator:AbstractValidator<SendCredentialsRequest>
    {
        public SendCredentialsValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.UniqueId));
        }
    }
}
