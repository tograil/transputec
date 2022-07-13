using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyTransaction
{
    public class GetCompanyTransactionResponse
    {
        public List<TransactionList> Data { get; set; }
        public string Message { get; set; }
    }
}
