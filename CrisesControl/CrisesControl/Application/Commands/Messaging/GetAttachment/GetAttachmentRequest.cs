using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetAttachment
{
    public class GetAttachmentRequest : IRequest<GetAttachmentResponse>
    {
        public int MessageAttachmentID { get; set; }
    }
}
