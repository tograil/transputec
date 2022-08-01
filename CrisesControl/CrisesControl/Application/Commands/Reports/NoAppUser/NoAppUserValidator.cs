using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Reports.NoAppUser
{
    public class NoAppUserValidator:AbstractValidator<NoAppUserRequest>
    {
        public NoAppUserValidator()
        {
            RuleFor(x=>x.CompanyId).GreaterThan(0);
            RuleFor(x => x.MessageID).GreaterThan(0);
            RuleFor(x => x.Draw).GreaterThan(0);
        }
    
    }
}
