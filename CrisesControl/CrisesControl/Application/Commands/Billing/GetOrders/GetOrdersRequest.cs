using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetOrders
{
    public class GetOrdersRequest:IRequest<GetOrdersResponse>
    {
        public int OrderId { get; set; }

    }
}
