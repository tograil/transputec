using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetOrders
{
    public class GetOrdersRequest:IRequest<GetOrdersResponse>
    {
        public int OrderId { get; set; }
        public int OriginalOrderId { get; set; }
        public int CompanyId { get; set; }
        public string CustomerId { get; set; }
    }
}
