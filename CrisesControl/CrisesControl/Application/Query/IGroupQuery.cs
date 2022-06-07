using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroups;
using CrisesControl.Api.Application.Commands.Groups.SegregationLinks;

namespace CrisesControl.Api.Application.Query
{
    public interface IGroupQuery
    {
        public Task<GetGroupsResponse> GetGroups(GetGroupsRequest request, CancellationToken cancellationToken);
        public Task<GetGroupResponse> GetGroup(GetGroupRequest request, CancellationToken cancellationToken);
        Task<SegregationLinksResponse> SegregationLinks(SegregationLinksRequest request);
    }
}
