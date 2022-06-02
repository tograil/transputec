using MediatR;

namespace CrisesControl.Api.Application.Commands.Register.CheckCustomer
{
    public class CheckCustomerRequest: IRequest<CheckCustomerResponse>
    {
        public string CustomerId { get; set; }
    }
}
