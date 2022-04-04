using Ardalis.GuardClauses;
using AutoMapper;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.GroupAggregate;
using CrisesControl.Core.GroupAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.UpdateGroup
{
    public class UpdateGroupHandler : IRequestHandler<UpdateGroupRequest, UpdateGroupResponse>
    {
        private readonly UpdateGroupValidator _groupValidator;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;

        public UpdateGroupHandler(UpdateGroupValidator groupValidator, IGroupRepository groupService, IMapper mapper)
        {
            _groupValidator = groupValidator;
            _groupRepository = groupService;
            _mapper = mapper;
        }

        public async Task<UpdateGroupResponse> Handle(UpdateGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateGroupRequest));

            Group value = _mapper.Map<UpdateGroupRequest, Group>(request);
            if (CheckForExistance(value))
            {
                var groupId = await _groupRepository.UpdateGroup(value, cancellationToken);
                var result = new UpdateGroupResponse();
                result.GroupId = groupId;
                return result;
            }
            return null;
        }

        private bool CheckForExistance(Group group)
        {
            return _groupRepository.CheckForExistance(group.GroupId);
        }
    }
}
