using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetUsageGraph
{
    public class GetUsageGraphResponse
    {
        public List<UsageGraph> Result { get; set; }
    }
}
