using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUserSystemInfo
{
    public class GetUserSystemInfoValidator : AbstractValidator<GetUserSystemInfoRequest>
    {
        public GetUserSystemInfoValidator()
        {
            RuleFor(x => x.QUserId)
                .GreaterThan(0);
        }
    }
}
