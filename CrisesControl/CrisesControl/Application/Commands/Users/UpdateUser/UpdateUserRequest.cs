using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.UpdateUser
{
    public class UpdateUserRequest : IRequest<UpdateUserResponse>
    {
        public int UserId { get; set; }  
        public int CompanyId { get; set; }
        public string? UserName { get; set; }
    }
}
