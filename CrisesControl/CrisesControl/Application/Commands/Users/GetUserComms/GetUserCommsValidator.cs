using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUserComms
{
    public class GetUserCommsValidator : AbstractValidator<GetUserCommsRequest>
    {
        public GetUserCommsValidator()
        {
            RuleFor(x => x.CommsUserId)
                .GreaterThan(0);
        }
    }
}
