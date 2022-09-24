using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Users.GetUserId
{
    public class GetUserIdValidator:AbstractValidator<GetUserIdRequest>
    {
        public GetUserIdValidator()
        {
            //RuleFor(x => x.CompanyId).GreaterThan(0);
            RuleFor(x => string.IsNullOrEmpty(x.EmailAddress));
        }
    }
}
