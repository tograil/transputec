using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask
{
    public class CompleteTaskHandler : IRequestHandler<CompleteTaskRequest, CompleteTaskResponse>
    {
        public CompleteTaskHandler()
        {

        }
        public Task<CompleteTaskResponse> Handle(CompleteTaskRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
