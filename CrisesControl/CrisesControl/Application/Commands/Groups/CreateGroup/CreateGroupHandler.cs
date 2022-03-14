using Ardalis.GuardClauses;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.DepartmentAggregate;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using CrisesControl.Core.GroupAggregate;
using CrisesControl.Core.GroupAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.CreateGroup
{
    public class CreateGroupHandler : IRequestHandler<CreateGroupRequest, CreateGroupResponse>
    {
        private readonly CreateGroupValidator _groupValidator;
        private readonly IGroupRepository _groupRepository;

        public CreateGroupHandler(CreateGroupValidator groupValidator, IGroupRepository groupRepository)
        {
            _groupValidator = groupValidator;
            _groupRepository = groupRepository;
        }

        public async Task<CreateGroupResponse> Handle(CreateGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(CreateGroupRequest));

            var sample = new Group();
            var groupId = await _groupRepository.CreateGroup(sample, cancellationToken);
            var result = new CreateGroupResponse();
            result.GroupId = groupId;
            return result;
        }
    }
}
