using AutoMapper;
using CrisesControl.Api.Application.Commands.Groups.CheckGroup;
using CrisesControl.Api.Application.Commands.Groups.GetAllGroup;
using CrisesControl.Api.Application.Commands.Groups.GetGroup;
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

        public async Task<GetAllGroupResponse> GetAllGroup(GetAllGroupRequest request, CancellationToken cancellationToken)
        {
            var groups = await _groupRepository.GetAllGroups(request.CompanyId, request.UserId, request.IncidentId);
            var result = new GetAllGroupResponse();
            result.Data = groups;
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
        public async Task<CheckGroupResponse> CheckGroup(CheckGroupRequest request)
        {
            var groups = await _groupRepository.DuplicateGroup(request.GroupName, request.CompanyId, request.GroupId, "Add");
            var result = _mapper.Map<bool>(groups);
            var response = new CheckGroupResponse();
            if (result)
            {
                response.Message= "Duplicate Group.";
            }
            else
            {
                response.Message = "No record found.";
            }
            return response;       
                   
        }
    }
}
