using MediatR;

namespace CrisesControl.Api.Application.Commands.Support.GetIncidentMessageAck
{
    public class GetIncidentMessageAckRequest : IRequest<GetIncidentMessageAckResponse>
    {
        public int MessageId { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageSentStatus { get; set; }
        public int RecordStart { get; set; }
        public int RecordLength { get; set; }
        public string search { get; set; }
        public string OrderBy { get; set; }
        public string OrderDir { get; set; }
        public int draw { get; set; }
        public string Filters { get; set; }
        public string CompanyKey { get; set; }
        public string Source { get; set; }
    }
}
