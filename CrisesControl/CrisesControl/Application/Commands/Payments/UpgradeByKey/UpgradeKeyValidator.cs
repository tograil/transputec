using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Payments.UpgradeByKey
{
    public class UpgradeKeyValidator : AbstractValidator<UpgradeByKeyRequest>
    {
        public UpgradeKeyValidator()
        {
            
            RuleFor(x => string.IsNullOrEmpty(x.ActivationKey));
        }
    }
}
