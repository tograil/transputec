using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Groups.GetAllGroup
{
    public class GetAllGroupHandler : IRequestHandler<GetAllGroupRequest, GetAllGroupResponse>
    {
        private readonly IGroupQuery _groupQuery;
        public GetAllGroupHandler(IGroupQuery groupQuery)
        {
            _groupQuery = groupQuery;
        }

        public Task<GetAllGroupResponse> Handle(GetAllGroupRequest request, CancellationToken cancellationToken)
        {
            var response = _groupQuery.GetAllGroup(request, cancellationToken);
            return response;
        }
    }
}
