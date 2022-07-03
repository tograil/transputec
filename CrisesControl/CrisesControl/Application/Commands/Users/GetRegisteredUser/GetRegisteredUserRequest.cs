using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetRegisteredUser
{
    public class GetRegisteredUserRequest : IRequest<GetRegisteredUserResponse>
    {
        public int QUserId { get; set; }
    }
}
