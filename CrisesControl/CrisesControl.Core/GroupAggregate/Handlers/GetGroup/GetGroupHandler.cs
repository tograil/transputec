using Ardalis.GuardClauses;
using CrisesControl.Core.GroupAggregate.Services;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.GroupAggregate.Handlers.GetGroup
{
    public class GetGroupHandler: IRequestHandler<GetGroupRequest, GetGroupResponse>
    {
        private readonly GetGroupValidator _groupValidator;
        private readonly IGroupService _groupService;

        public GetGroupHandler(GetGroupValidator groupValidator, IGroupService groupService)
        {
            _groupService = groupService;
            _groupValidator = groupValidator;
        }
        public async Task<GetGroupResponse> Handle(GetGroupRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GetGroupRequest));

            await _groupValidator.ValidateAndThrowAsync(request, cancellationToken);

            var groups = await _groupService.GetAllGroups();

            return new GetGroupResponse();
        }
    }
}
