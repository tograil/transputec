using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetTransactionDetails
{
    public class GetTransactionDetailsRequest : IRequest<GetTransactionDetailsResponse>
    {
        public int companyId { get; set; }
        public int messageId { get; set; }
        public string method { get; set; }
        public int recordStart { get; set; }
        public int recordLength { get; set; }
        public string searchString { get; set; }
        public string orderBy { get; set; }
        public string orderDir { get; set; }
        public string companyKey { get; set; }
    }
}
