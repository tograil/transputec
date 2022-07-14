using CrisesControl.Core.Administrator;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddTransaction
{
    public class AddTransactionRequest:IRequest<AddTransactionResponse>
    {
        public UpdateTransactionDetailsModel IP { get; set; }
    }
}
