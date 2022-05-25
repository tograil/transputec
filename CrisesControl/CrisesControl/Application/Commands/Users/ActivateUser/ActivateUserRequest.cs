using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ActivateUser
{
    public class ActivateUserRequest:IRequest<ActivateUserResponse>
    {
        public int QueriedUserId { get; set; }
    }
}
