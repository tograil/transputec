using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetUsageGraph
{
    public class GetUsageGraphResponse
    {
        public string MethodName { get; set; }
        public string UsageMonth { get; set; }
        public int RptMonth { get; set; }
        public int RptYear { get; set; }
        public decimal Total { get; set; }
        public int Quantity { get; set; }
    }
}
