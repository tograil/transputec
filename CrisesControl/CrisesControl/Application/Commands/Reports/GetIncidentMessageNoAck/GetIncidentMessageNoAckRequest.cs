using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageNoAck
{
    public class GetIncidentMessageNoAckRequest: IRequest<GetIncidentMessageNoAckResponse>
    {
        public int IncidentActivationId { get; set; }
        public int RecordStart { get; set; }
        public int RecordLength { get; set; }
        public string? SearchString { get; set; }
        public string? UniqueKey { get; set; }
        public int draw { get; set; }

    }
}
