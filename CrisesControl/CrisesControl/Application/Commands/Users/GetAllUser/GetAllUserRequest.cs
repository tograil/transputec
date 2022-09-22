using CrisesControl.Core.Users;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.GetAllUser
{
    public class GetAllUserRequest : IRequest<GetAllUserResponse>
    {
        public string SearchString { get; set; }
        public string OrderDir { get; set; }
        public bool SkipDeleted { get; set; }
        public bool ActiveOnly { get; set; }
        public bool SkipInActive { get; set; }
        public bool KeyHolderOnly { get; set; }
        public string Filters { get; set; }
        public string CompanyKey { get; set; }
    }
}
