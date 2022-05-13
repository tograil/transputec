using CrisesControl.Core.Reports;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck
{
    public class GetIndidentMessageAckRequest : IRequest<GetIndidentMessageAckResponse>
    {
       
        public int MessageId { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageSentStatus { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string? SearchString { get; set; }
        public List<Order>? order { get; set; }
        public int? draw { get; set; }
        public string? Filters { get; set; }
        public string? CompanyKey { get; set; }
        public string? Source { get; set; }

    }

    public class IncidentMsgAckRequestRoute 
    {
        public int MessageId { get; set; }
        public int MessageAckStatus { get; set; }
        public int MessageSentStatus { get; set; }
    }

    public class IncidentMsgAckRequestQuery : IRequest<GetIndidentMessageAckResponse>
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public string? SearchString { get; set; }
        public List<Order>? order { get; set; }
        public int? draw { get; set; }
        public string? Filters { get; set; }
        public string? CompanyKey { get; set; }
        public string? Source { get; set; }
    }
}
