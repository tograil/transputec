using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.SendVerification
{
    public class SendVerificationValidator:AbstractValidator<SendVerificationRequest>
    {
        public SendVerificationValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.UniqueId));
        }
    }
}
