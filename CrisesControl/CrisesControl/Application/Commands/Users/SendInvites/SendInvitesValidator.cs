using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.SendInvites
{
    public class SendInvitesValidator: AbstractValidator<SendInvitesRequest>
    {
        public SendInvitesValidator()
        {
            RuleFor(x => x.CompanyProfile)
                .NotEmpty();
            RuleFor(x => x.Status)
                .NotEmpty();

        }
    }
}
