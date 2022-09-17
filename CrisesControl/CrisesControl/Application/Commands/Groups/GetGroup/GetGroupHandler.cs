using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetGroup
{
    public class GetGroupHandler : IRequestHandler<GetGroupRequest, GetGroupResponse>
    {
        private readonly IGroupQuery _groupQuery;
        public GetGroupHandler(IGroupQuery groupQuery)
        {
            _groupQuery = groupQuery;
        }

        public async Task<GetGroupResponse> Handle(GetGroupRequest request, CancellationToken cancellationToken)
        {
            var response = await _groupQuery.GetGroup(request, cancellationToken);
            return response;
        }
    }
}
