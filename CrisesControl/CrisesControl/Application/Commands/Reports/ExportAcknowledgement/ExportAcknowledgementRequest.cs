using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.ExportAcknowledgement
{
    public class ExportAcknowledgementRequest:IRequest<ExportAcknowledgementResponse>
    {
        public int MessageId { get; set; }
        public string CompanyKey { get; set; }
    }
}
