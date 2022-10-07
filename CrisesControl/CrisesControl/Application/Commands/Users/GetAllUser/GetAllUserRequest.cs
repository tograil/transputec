using CrisesControl.Core.Users;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUser
{
    public class GetAllUserRequest : IRequest<GetAllUserResponse>
    {
        public int CompanyId { get; set; }
        public bool SkipDeleted { get; set; }
        public bool ActiveOnly { get; set; }
        public bool SkipInActive { get; set; }
        public bool KeyHolderOnly { get; set; }
    }
}
