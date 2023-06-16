using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.UpgradeRequest
{
    public class UpgradeRequestValidator : AbstractValidator<UpgradeRequest>
    {
        public UpgradeRequestValidator()
        {
            RuleFor(x => x.CompanyId).GreaterThan(0);
        }
    }
}
