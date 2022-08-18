using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Security;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.UpdateSecurityGroup
{
    public class UpdateSecurityGroupHandler : IRequestHandler<UpdateSecurityGroupRequest, UpdateSecurityGroupResponse>
    {
        private readonly ISecurityRepository _securityRepository;
        private readonly ICurrentUser _currentUser;
        public UpdateSecurityGroupHandler(ISecurityRepository securityRepository, ICurrentUser currentUser)
        {
            this._securityRepository = securityRepository;
            this._currentUser = currentUser;
        }
        public async Task<UpdateSecurityGroupResponse> Handle(UpdateSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            var securityId = await _securityRepository.UpdateSecurityGroup(_currentUser.CompanyId, request.SecurityGroupId, request.GroupName, request.GroupDescription, request.Status,
                        request.UserRole, request.GroupSecurityObjects, _currentUser.UserId, _currentUser.TimeZone);
            var response = new UpdateSecurityGroupResponse();
            if (securityId >0)
            {
                response.SecurityGroupId = securityId;
            }
            else
            {
                response.SecurityGroupId = 0;
            }
            return response;
        }
    }
}
