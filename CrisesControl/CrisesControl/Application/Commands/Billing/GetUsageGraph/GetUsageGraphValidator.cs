using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Billing.GetUsageGraph
{
    public class GetUsageGraphValidator:AbstractValidator<GetUsageGraphRequest>
    {
        public GetUsageGraphValidator()
        {
            RuleFor(x => x.LastMonth).GreaterThan(0);
        }
    }
}
