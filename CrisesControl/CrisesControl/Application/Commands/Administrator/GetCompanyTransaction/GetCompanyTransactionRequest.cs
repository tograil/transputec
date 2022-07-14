using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetCompanyTransaction
{
    public class GetCompanyTransactionRequest:IRequest<GetCompanyTransactionResponse>
    {
        public int CompanyId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
    }
}
