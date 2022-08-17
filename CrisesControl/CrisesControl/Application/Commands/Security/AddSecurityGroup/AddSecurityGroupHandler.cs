using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Security;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.AddSecurityGroup
{
    public class AddSecurityGroupHandler : IRequestHandler<AddSecurityGroupRequest, AddSecurityGroupResponse>
    {
        private readonly ISecurityRepository _securityRepository;
        private readonly ICurrentUser _currentUser;
        public AddSecurityGroupHandler(ISecurityRepository securityRepository, ICurrentUser currentUser)
        {
            this._securityRepository = securityRepository;
            this._currentUser = currentUser;
        }
        public async Task<AddSecurityGroupResponse> Handle(AddSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var securityId = await _securityRepository.AddSecurityGroup(_currentUser.CompanyId, request.GroupName, request.GroupDescription, request.Status,
                        request.UserRole, request.GroupSecurityObjects, _currentUser.UserId, _currentUser.TimeZone);
                var response = new AddSecurityGroupResponse();
                if (securityId > 0)
                {
                    response.SecurityGroupId = securityId;
                }
                else
                {
                    response.SecurityGroupId = 0;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
