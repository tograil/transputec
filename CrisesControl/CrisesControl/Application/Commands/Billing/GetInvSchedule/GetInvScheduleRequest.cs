using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvSchedule
{
    public class GetInvScheduleRequest : IRequest<GetInvScheduleResponse>
    {
        public int OrderId { get; set; }
        public int MonthVal { get; set; }
        public int YearVal { get; set; }

    }
}
