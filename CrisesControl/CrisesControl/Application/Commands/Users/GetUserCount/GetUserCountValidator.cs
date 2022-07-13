using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUserCount
{
    public class GetUserCountValidator : AbstractValidator<GetUserCountRequest>
    {
        public GetUserCountValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
        }
    }
}
