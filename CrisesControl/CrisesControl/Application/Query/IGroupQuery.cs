using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroups;

namespace CrisesControl.Api.Application.Query
{
    public interface IGroupQuery
    {
        public Task<GetGroupsResponse> GetGroups(GetGroupsRequest request);
        public Task<GetGroupResponse> GetGroup(GetGroupRequest request);
    }
}
