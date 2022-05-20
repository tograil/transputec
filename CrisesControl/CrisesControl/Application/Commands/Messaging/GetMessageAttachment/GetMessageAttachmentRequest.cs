using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageAttachment
{
    public class GetMessageAttachmentRequest : IRequest<GetMessageAttachmentResponse>
    {
       public  int MessageListID { get; set; } 
       public int MessageID { get; set; }
    }
}
