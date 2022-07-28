using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAttachments
{
    public class GetAttachmentsRequest:IRequest<GetAttachmentsResponse>
    {
        public int ObjectId { get; set; }
        public string AttachmentsType { get; set; }
    }
}
