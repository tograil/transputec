using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.GetTempRegistration
{
    public class GetTempRegistrationValidator : AbstractValidator<GetTempRegistrationRequest>
    {
        public GetTempRegistrationValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.UniqueRef));
            RuleFor(x => x.RegId).GreaterThan(0);
        }
    }
}
