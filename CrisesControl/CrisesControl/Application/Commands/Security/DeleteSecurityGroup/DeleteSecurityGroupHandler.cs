using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Security;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Security.DeleteSecurityGroup
{
    public class DeleteSecurityGroupHandler : IRequestHandler<DeleteSecurityGroupRequest, DeleteSecurityGroupResponse>
    {
        private readonly ISecurityRepository _securityRepository;
        private readonly ICurrentUser _currentUser;
        public DeleteSecurityGroupHandler(ISecurityRepository securityRepository, ICurrentUser currentUser)
        {
            this._securityRepository = securityRepository;
            this._currentUser = currentUser;
        }
        public async Task<DeleteSecurityGroupResponse> Handle(DeleteSecurityGroupRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var deleted = await _securityRepository.DeleteSecurityGroup(_currentUser.CompanyId, request.SecurityGroupId, _currentUser.UserId, _currentUser.TimeZone);
                var response = new DeleteSecurityGroupResponse();
                if (deleted)
                {
                    response.Deleted = deleted;
                    response.Message = "Security Group has been deleted";
                }
                else
                {
                    response.Deleted = false;
                    response.Message = "No record Found.";
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
