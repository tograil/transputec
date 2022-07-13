using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.SendPasswordOTP
{
    public class SendPasswordOTPValidator : AbstractValidator<SendPasswordOTPRequest>
    {
        public SendPasswordOTPValidator()
        {
        }
    }
}
