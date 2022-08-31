using Ardalis.GuardClauses;
using CrisesControl.Core.Groups.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GroupUpdateSegregationLink
{
    public class UpdateSegregationLinkHandler : IRequestHandler<GroupUpdateSegregationLinkRequest, GroupUpdateSegregationLinkResponse>
    {
        private readonly IGroupRepository _groupRepository;
        public UpdateSegregationLinkHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<GroupUpdateSegregationLinkResponse> Handle(GroupUpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(GroupUpdateSegregationLinkRequest));
            var response = new GroupUpdateSegregationLinkResponse();
            response.Data = await _groupRepository.UpdateSegregationLink(request.SourceId, request.TargetId, request.Action, request.LinkType);
            return response;
        }
    }
}
