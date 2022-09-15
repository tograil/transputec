using CrisesControl.Core.Users;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUser
{
    public class GetAllUserRequest : IRequest<GetAllUserResponse>
    {
        public GetAllUserRequestList GetAllUserRequestList { get; set; }
    }
}
