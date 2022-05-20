using CrisesControl.Core.Models;
using System.Net;

namespace CrisesControl.Api.Application.Commands.Messaging.GetMessageAttachment
{
    public class GetMessageAttachmentResponse
    {
        public List<MessageAttachment> Data { get; set; }
        public HttpStatusCode ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
