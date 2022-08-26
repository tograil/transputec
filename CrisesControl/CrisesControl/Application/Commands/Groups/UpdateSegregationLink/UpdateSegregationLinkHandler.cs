using Ardalis.GuardClauses;
using CrisesControl.Core.Groups.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.UpdateSegregationLink
{
    public class UpdateSegregationLinkHandler : IRequestHandler<UpdateSegregationLinkRequest, UpdateSegregationLinkResponse>
    {
        private readonly IGroupRepository _groupRepository;
        public UpdateSegregationLinkHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<UpdateSegregationLinkResponse> Handle(UpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(UpdateSegregationLinkRequest));
            var response = new UpdateSegregationLinkResponse();
            response.Data = await _groupRepository.UpdateSegregationLink(request.SourceId, request.TargetId, request.Action, request.LinkType);
            return response;
        }
    }
}
