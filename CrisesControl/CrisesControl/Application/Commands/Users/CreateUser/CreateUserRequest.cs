using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.CreateUser
{
    public class CreateUserRequest : IRequest<CreateUserResponse>
    {
        public int UserId { get; set; }  
        public int CompanyId { get; set; }
        public string? UserName { get; set; }
    }
}
