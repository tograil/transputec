using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Register.CheckCustomer
{
    public class CheckCustomerValidator : AbstractValidator<CheckCustomerRequest>
    {
        public CheckCustomerValidator()
        {
            RuleFor(x => string.IsNullOrEmpty(x.CustomerId));
        }
    }
}
