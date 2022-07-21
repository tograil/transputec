using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUserMovements
{
    public class GetUserMovementsValidator : AbstractValidator<GetUserMovementsRequest>
    {
        public GetUserMovementsValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
        }
    }
}
