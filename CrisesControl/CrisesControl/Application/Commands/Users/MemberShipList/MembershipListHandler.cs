using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Users.MembershipList
{
    public class MembershipListHandler : IRequestHandler<MembershipRequest, MembershipResponse>
    {
        private readonly IUserQuery _userQuery;
        public MembershipListHandler(IUserQuery userQuery)
        {
         _userQuery= userQuery;
        }
        public async Task<MembershipResponse> Handle(MembershipRequest request, CancellationToken cancellationToken)
        {
            var usersMember = await _userQuery.MembershipList(request);
            return usersMember;
        }
    }
}
