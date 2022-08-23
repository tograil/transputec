using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.AttachSOPToIncident
{
    public class AttachSOPToIncidentRequest:IRequest<AttachSOPToIncidentResponse>
    {
        public int SOPHeaderID { get; set; }
        public string SOPFileName { get; set; }
    }
}
