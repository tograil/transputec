using MediatR;

namespace CrisesControl.Api.Application.Commands.Reports.AppInvitation
{
    public class AppInvitationRequest:IRequest<AppInvitationResponse>
    {
        public int CompanyId { get; set; }
    }
}
