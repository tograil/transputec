using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Core.Groups;
using CrisesControl.Core.Groups.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.CreateGroup
{
    public class CreateGroupHandler : IRequestHandler<CreateGroupRequest, CreateGroupResponse>
    {
        private readonly CreateGroupValidator _groupValidator;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public CreateGroupHandler(CreateGroupValidator groupValidator, IGroupRepository groupRepository, IMapper mapper)
        {
            _groupValidator = groupValidator;
            _groupRepository = groupRepository;
            _mapper = mapper;
        }

        public async Task<CreateGroupResponse> Handle(CreateGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateGroupRequest));

            Group value = _mapper.Map<CreateGroupRequest, Group>(request);
            if (!CheckDuplicate(value))
            {
                var groupId = await _groupRepository.CreateGroup(value, cancellationToken);
                var result = new CreateGroupResponse();
                result.GroupId = groupId;
                return result;
            }
            return null;
        }

        private bool CheckDuplicate(Group group)
        {
            return _groupRepository.CheckDuplicate(group);
        }
    }
}
