using MediatR;

namespace CrisesControl.Api.Application.Commands.Messaging.GetPublicAlertTemplate
{
    public class GetPublicAlertTemplateRequest:IRequest<GetPublicAlertTemplateResponse>
    {
        public int MessageId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
    }
}
