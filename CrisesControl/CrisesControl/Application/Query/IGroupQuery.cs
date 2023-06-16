using CrisesControl.Api.Application.Commands.Groups.CheckGroup;
using CrisesControl.Api.Application.Commands.Groups.GetAllGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Commands.Groups.SegregationLinks;

namespace CrisesControl.Api.Application.Query
{
    public interface IGroupQuery
    {
        public Task<GetAllGroupResponse> GetAllGroup(GetAllGroupRequest request, CancellationToken cancellationToken);
        public Task<GetGroupResponse> GetGroup(GetGroupRequest request, CancellationToken cancellationToken);
        Task<SegregationLinksResponse> SegregationLinks(SegregationLinksRequest request);
        Task<CheckGroupResponse> CheckGroup(CheckGroupRequest request);
    }
}
