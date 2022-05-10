using CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup;

namespace CrisesControl.Api.Application.Query
{
    public interface ISecurityQuery
    {
        public Task<GetCompanySecurityGroupResponse> GetCompanySecurityGroup(GetCompanySecurityGroupRequest request);
    }
}
