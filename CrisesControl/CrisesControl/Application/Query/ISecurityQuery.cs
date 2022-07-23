using CrisesControl.Api.Application.Commands.Security.AddSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.DeleteSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.GetAllSecurityObjects;
using CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup;
using CrisesControl.Api.Application.Commands.Security.GetSecurityGroup;
using CrisesControl.Api.Application.Commands.Security.UpdateSecurityGroup;

namespace CrisesControl.Api.Application.Query
{
    public interface ISecurityQuery
    {
        public Task<GetCompanySecurityGroupResponse> GetCompanySecurityGroup(GetCompanySecurityGroupRequest request);
        public Task<GetSecurityGroupResponse> GetSecurityGroup(GetSecurityGroupRequest request);
        public Task<AddSecurityGroupResponse> AddSecurityGroup(AddSecurityGroupRequest request);
        public Task<UpdateSecurityGroupResponse> UpdateSecurityGroup(UpdateSecurityGroupRequest request);
        public Task<DeleteSecurityGroupResponse> DeleteSecurityGroup(DeleteSecurityGroupRequest request);
        public Task<GetAllSecurityObjectsResponse> GetAllSecurityObjects(GetAllSecurityObjectsRequest request);
    }
}
