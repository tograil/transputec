using MediatR;

namespace CrisesControl.Api.Application.Commands.App.ReferToFriend
{
    public class ReferToFriendRequest:IRequest<ReferToFriendResponse>
    {
        public string ReferToName { get; set; }

        public string ReferToEmail { get; set; }
        public string ReferMessage { get; set; }
        public string UserEmail { get; set; }
    }
}
