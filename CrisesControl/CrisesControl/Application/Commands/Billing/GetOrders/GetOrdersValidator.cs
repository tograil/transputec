using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Billing.GetOrders
{
    public class GetOrdersValidator:AbstractValidator<GetOrdersRequest>
    {
        public GetOrdersValidator()
        {
            RuleFor(x => x.OrderId).GreaterThan(0);
            RuleFor(x => x.OriginalOrderId).GreaterThan(0);
            RuleFor(x =>string.IsNullOrEmpty( x.CustomerId));
        }
    }
}
