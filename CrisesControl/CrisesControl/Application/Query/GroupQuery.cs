using AutoMapper;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroups;
using CrisesControl.Core.GroupAggregate;
using CrisesControl.Core.GroupAggregate.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class GroupQuery: IGroupQuery
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        public GroupQuery(IGroupRepository groupRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<GetGroupsResponse> GetGroups(GetGroupsRequest request)
        {
            var groups = await _groupRepository.GetAllGroups(request.CompanyId);
            List<GetGroupResponse> response = _mapper.Map<List<GetGroupResponse>>(groups.ToList());
            var result = new GetGroupsResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetGroupResponse> GetGroup(GetGroupRequest request)
        {
            var group = await _groupRepository.GetGroup(request.CompanyId, request.GroupId);
            GetGroupResponse response = _mapper.Map<GetGroupResponse>(group);

            return response;
        }
    }
}
