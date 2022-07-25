using MediatR;

namespace CrisesControl.Api.Application.Commands.Billing.GetUnbilledSummary
{
    public class GetUnbilledSummaryRequest : IRequest<GetUnbilledSummaryResponse>
    {
        public int StartYear { get; set; }
        public int StartMonth { get; set; }
        public int MessageId { get; set; }
        public string ReportType { get; set; }
    }
}
