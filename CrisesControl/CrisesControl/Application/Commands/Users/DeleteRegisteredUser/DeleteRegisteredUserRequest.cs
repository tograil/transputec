using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.DeleteRegisteredUser
{
    public class DeleteRegisteredUserRequest : IRequest<DeleteRegisteredUserResponse>
    {
        public int CustomerId { get; set; }
        public string UniqueGUID { get; set; }
    }
}
