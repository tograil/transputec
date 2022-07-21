using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetBillingSummary
{
    public class GetBillingSummaryRequest : IRequest<GetBillingSummaryResponse>
    {
        public int OutUserCompanyId { get; set; }
        public int ChkUserId { get; set; }
    }
}
