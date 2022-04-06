using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetUser
{
    public class GetUserRequest : IRequest<GetUserResponse>
    {
        public int UserId { get; set; }  
        public int CompanyId { get; set; }
    }
}
