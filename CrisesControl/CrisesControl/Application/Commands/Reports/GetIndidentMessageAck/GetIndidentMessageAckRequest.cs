using CrisesControl.Core.Reports.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck
{
    public class GetIndidentMessageAckRequest: IRequest<GetIndidentMessageAckResponse>
    {
        public int MessageId { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageSentStatus { get; set; }
        public int RecordStart { get; set; }
        public int RecordLength { get; set; }
        public string SearchString { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }
        public int CurrentUserId { get; set; }
        public string Filters { get; set; }
        public string CompanyKey { get; set; }            
        public string Source { get; set; }
    }
}
