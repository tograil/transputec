using FluentValidation;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvSchedule
{
    public class GetInvScheduleValidator:AbstractValidator<GetInvScheduleRequest>
    {
        public GetInvScheduleValidator()
        {
            RuleFor(x => x.MonthVal).GreaterThan(0);
            RuleFor(x => x.OrderId).GreaterThan(0);
            RuleFor(x => x.YearVal).GreaterThan(0);
        }
    }
}
