using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageAck
{
    public class GetIncidentMessageAckRequest : IRequest<GetIncidentMessageAckResponse>
    {
       
        public int MessageId { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageSentStatus { get; set; }       
        public string? SearchString { get; set; }
        public string? Source { get; set; }

    }

    public class IncidentMsgAckRequestRoute 
    {
        public int MessageId { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageSentStatus { get; set; }
    }

    public class IncidentMsgAckRequestQuery : IRequest<GetIncidentMessageAckResponse>
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public string? SearchString { get; set; }
        public string OrderDir { get; set; }
        public int? draw { get; set; }
        public string? Filters { get; set; }
        public string? CompanyKey { get; set; }
        public string? Source { get; set; }
    }
}
