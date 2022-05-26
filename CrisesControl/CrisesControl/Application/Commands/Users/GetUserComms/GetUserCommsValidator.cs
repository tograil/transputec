using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUserComms
{
    public class GetUserCommsValidator : AbstractValidator<GetUserCommsRequest>
    {
        public GetUserCommsValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
            RuleFor(x => x.CompanyId)
                .GreaterThan(0);

        }
    }
}
