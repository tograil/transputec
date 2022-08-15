using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetUnpaidTransactions
{
    public class GetUnpaidTransactionsRequest:IRequest<GetUnpaidTransactionsResponse>
    {
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
