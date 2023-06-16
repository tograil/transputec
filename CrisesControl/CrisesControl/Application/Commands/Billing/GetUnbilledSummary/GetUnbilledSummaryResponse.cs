using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary
{
    public class GetUnbilledSummaryResponse
    {
        public List<UnbilledSummary> unbilledSummaries { get; set; }
    }
}
