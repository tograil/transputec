using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Security;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.GetSecurityGroup
{

    public class GetSecurityGroupHandler : IRequestHandler<GetSecurityGroupRequest, GetSecurityGroupResponse>
    {
        private readonly ISecurityRepository _securityRepository;
        private readonly ICurrentUser _currentUser;
        public GetSecurityGroupHandler(ISecurityRepository securityRepository, ICurrentUser currentUser)
        {
            this._securityRepository = securityRepository;
            this._currentUser = currentUser;
        }
        public async Task<GetSecurityGroupResponse> Handle(GetSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var security = await _securityRepository.GetSecurityGroup(_currentUser.CompanyId, request.SecurityGroupId);
            var response = new GetSecurityGroupResponse();
            if (security !=null)
            {
                response.Data = security;
            }
            else
            {
                response.Data = null;
            }
            return response;
        }
    }
}
