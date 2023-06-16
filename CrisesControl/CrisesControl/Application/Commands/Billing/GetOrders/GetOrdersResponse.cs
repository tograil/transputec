using CrisesControl.Core.Billing;

namespace CrisesControl.Api.Application.Commands.Billing.GetOrders
{
    public class GetOrdersResponse
    {
        public List<OrderListReturn> Data { get; set; }
    }
}
