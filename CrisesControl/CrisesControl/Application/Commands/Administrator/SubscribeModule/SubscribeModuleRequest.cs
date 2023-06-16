using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.SubscribeModule
{
    public class SubscribeModuleRequest:IRequest<SubscribeModuleResponse>
    {
        public int TransactionTypeId { get; set; }
        public string PaymentPeriod { get; set; }
    }
}
