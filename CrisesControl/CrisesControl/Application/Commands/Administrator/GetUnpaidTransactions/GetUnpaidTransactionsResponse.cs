using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetUnpaidTransactions
{
    public class GetUnpaidTransactionsResponse
    {
        public List<UnpaidTransaction> Data { get; set; }
        public string Message { get; set; }
    }
}
