using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Administrator.SubscribeModule
{
    public class SubscribeModuleValidator:AbstractValidator<SubscribeModuleRequest>
    {
        public SubscribeModuleValidator()
        {
            RuleFor(x => x.TransactionTypeId).GreaterThan(0);
            RuleFor(x => string.IsNullOrEmpty(x.PaymentPeriod));
        }
    }
}
