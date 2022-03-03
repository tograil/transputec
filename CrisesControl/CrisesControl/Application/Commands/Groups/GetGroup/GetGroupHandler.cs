using Ardalis.GuardClauses;
using CrisesControl.Core.DepartmentAggregate.Repositories;
using CrisesControl.Core.GroupAggregate.Repositories;
using FluentValidation;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroup
{
    public class GetGroupHandler: IRequestHandler<GetGroupRequest, GetGroupResponse>
    {
        private readonly GetGroupValidator _groupValidator;
        private readonly IGroupRepository _groupRepository;

        public GetGroupHandler(GetGroupValidator groupValidator, IGroupRepository groupRepository)
        {
            _groupValidator = groupValidator;
            _groupRepository = groupRepository;
        }

        public async Task<GetGroupResponse> Handle(GetGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetGroupRequest));
            
            await _groupValidator.ValidateAndThrowAsync(request, cancellationToken);
            
            var departments = await _groupRepository.GetAllGroups();

            return new GetGroupResponse();
        }
    }
}
