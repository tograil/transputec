using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.IncidentResponseDump
{
    public class IncidentResponseDumpRequest:IRequest<IncidentResponseDumpResponse>
    {
        public int IncidentActivationId { get; set; }
    }
}
