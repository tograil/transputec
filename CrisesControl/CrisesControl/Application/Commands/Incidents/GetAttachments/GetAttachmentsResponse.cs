using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAttachments
{
    public class GetAttachmentsResponse
    {
        public List<Attachment> result { get; set; }
        public string Message { get; set; }
    }
}
