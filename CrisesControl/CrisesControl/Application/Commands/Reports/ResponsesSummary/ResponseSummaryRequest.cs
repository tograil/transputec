using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.ResponsesSummary
{
    public class ResponseSummaryRequest : IRequest<ResponseSummaryResponse>
    {
        public int MessageId { get; set; }
    }
}
