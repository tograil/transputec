using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary
{
    public class GetMessageDeliverySummaryRequest :IRequest<GetMessageDeliverySummaryResponse>
    {
        public int MessageID { get; set; }
    }
}
