using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetInvSchedule
{
    public class GetInvScheduleResponse
    {
        public List<InvoiceSchReturn> Data { get; set; }
    }
}
