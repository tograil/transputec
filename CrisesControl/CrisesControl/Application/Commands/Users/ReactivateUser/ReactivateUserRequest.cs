using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.ReactivateUser
{
    public class ReactivateUserRequest : IRequest<ReactivateUserResponse>
    {
        public int QueriedUserId { get; set; }
        public string Filters { get; set; }
    }
}
