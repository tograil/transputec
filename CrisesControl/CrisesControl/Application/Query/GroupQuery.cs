using AutoMapper;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroups;
using CrisesControl.Api.Application.Commands.Groups.SegregationLinks;
using CrisesControl.Core.Groups;
using CrisesControl.Core.Groups.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class GroupQuery: IGroupQuery
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupQuery> _logger;
        public GroupQuery(IGroupRepository groupRepository, IMapper mapper, ILogger<GroupQuery> logger)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
            _logger = logger;   
        }

        public async Task<GetGroupsResponse> GetGroups(GetGroupsRequest request, CancellationToken cancellationToken)
        {
            var groups = await _groupRepository.GetAllGroups(request.CompanyId);
            List<GetGroupResponse> response = _mapper.Map<List<GetGroupResponse>>(groups.ToList());
            var result = new GetGroupsResponse();
            result.Data = response;
            return result;
        }

        public async Task<GetGroupResponse> GetGroup(GetGroupRequest request, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetGroup(request.CompanyId, request.GroupId);
            GetGroupResponse response = _mapper.Map<GetGroupResponse>(group);

            return response;
        }

        public async Task<SegregationLinksResponse> SegregationLinks(SegregationLinksRequest request)
        {
            var groups = await _groupRepository.SegregationLinks(request.TargetID, request.MemberShipType,request.LinkType);
            var response = _mapper.Map<List<GroupLink>>(groups);
            var result = new SegregationLinksResponse();
            result.data = response;
            result.Message = "Data Loaded Succesfully";
            return result;
        }
    }
}
