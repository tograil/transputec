using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.UploadAttachment
{
    public class UploadAttachmentRequest: IRequest<UploadAttachmentResponse>
    {
        public int ErrorId { get; set; }
        public string ErrorCode { get; set; }
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public int ResultID { get; set; }
    }
}
