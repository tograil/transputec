using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetKeyHolders
{
    public class GetKeyHoldersValidator : AbstractValidator<GetKeyHoldersRequest>
    {
        public GetKeyHoldersValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
        }
    }
}
