using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Administrator.GetTransactionType
{
    public class GetTransactionTypeResponse
    {
        public List<TransactionType> Data { get; set; }
        public string Message { get; set; }
    }
}
