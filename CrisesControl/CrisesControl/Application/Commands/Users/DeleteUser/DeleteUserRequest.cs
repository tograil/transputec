using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.DeleteUser
{
    public class DeleteUserRequest : IRequest<DeleteUserResponse>
    {
        public int UserId { get; set; }  
        public int CompanyId { get; set; }
        public string? UserName { get; set; }
    }
}
