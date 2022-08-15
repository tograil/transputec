using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateCustomer
{
    public class UpdateCustomerRequest:IRequest<UpdateCustomerResponse>
    {
        public string NewCustomerId { get; set; }
        public int QCompanyId { get; set; }
        public string QCustomerId { get; set; }
    }
}
