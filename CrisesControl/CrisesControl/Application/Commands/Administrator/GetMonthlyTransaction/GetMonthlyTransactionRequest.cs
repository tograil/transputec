using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetMonthlyTransaction
{
    public class GetMonthlyTransactionRequest:IRequest<GetMonthlyTransactionResponse>
    {
    }
}
