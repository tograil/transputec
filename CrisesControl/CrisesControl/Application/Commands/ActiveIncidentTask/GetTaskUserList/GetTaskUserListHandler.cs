using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskUserList
{
    public class GetTaskUserListHandler : IRequestHandler<GetTaskUserListRequest, GetTaskUserListResponse>
    {
        public GetTaskUserListHandler()
        {

        }
        public Task<GetTaskUserListResponse> Handle(GetTaskUserListRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
