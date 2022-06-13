using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.SendInvites
{
    public class SendInvitesRequest : IRequest<SendInvitesResponse>
    {
        public string CompanyProfile { get; set; }
        public int Status { get; set; }
    }
}
