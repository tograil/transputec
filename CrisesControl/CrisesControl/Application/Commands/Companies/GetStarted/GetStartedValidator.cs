using CrisesControl.Api.Application.Commands.GetStarted;
using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Companies.GetStarted
{
    public class GetStartedValidator:AbstractValidator<GetStartedRequest>
    {
        public GetStartedValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
        }
    }
}
