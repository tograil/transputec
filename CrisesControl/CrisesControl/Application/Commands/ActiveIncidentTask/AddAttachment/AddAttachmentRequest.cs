using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddAttachment
{
    public class AddAttachmentRequest:IRequest<AddAttachmentResponse>
    {
        public int ActiveIncidentTaskID { get; set; }
        public string AttachmentTitle { get; set; }
        public string FileName { get; set; }
        public string SourceFileName { get; set; }
        public double FileSize { get; set; }
    }
}
