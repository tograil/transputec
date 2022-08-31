using MediatR;

namespace CrisesControl.Api.Application.Commands.Support.GetUser
{
    public class GetUserRequest : IRequest<GetUserResponse>
    {
        public int UserId { get; set; }
    }
}
