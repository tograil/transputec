using CrisesControl.Core.Administrator;

namespace CrisesControl.Api.Application.Commands.Administrator.GetMonthlyTransaction
{
    public class GetMonthlyTransactionResponse
    {
        public List<AdminTransaction> Data { get; set; }
    }
}
