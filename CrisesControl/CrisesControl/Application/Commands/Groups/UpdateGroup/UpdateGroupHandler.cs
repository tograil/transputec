using Ardalis.GuardClauses;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.DepartmentAggregate;
using CrisesControl.Core.DepartmentAggregate.Repositories;
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

        public UpdateGroupHandler(UpdateGroupValidator groupValidator, IGroupRepository groupService)
        {
            _groupValidator = groupValidator;
            _groupRepository = groupService;
        }

        public async Task<UpdateGroupResponse> Handle(UpdateGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateGroupRequest));

            var sample = new Group();
            var groupId = await _groupRepository.UpdateGroup(sample, cancellationToken);
            var result = new UpdateGroupResponse();
            result.GroupId = groupId;
            return result;
        }
    }
}
