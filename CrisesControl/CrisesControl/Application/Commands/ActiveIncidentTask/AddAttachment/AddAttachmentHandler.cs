using Ardalis.GuardClauses;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddAttachment
{
    public class AddAttachmentHandler : IRequestHandler<AddAttachmentRequest, AddAttachmentResponse>
    {
        private readonly IActiveIncidentRepository _activeIncidentRepository;
        public AddAttachmentHandler(IActiveIncidentRepository activeIncidentRepository)
        {
            _activeIncidentRepository = activeIncidentRepository;
        }

        public async Task<AddAttachmentResponse> Handle(AddAttachmentRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(AddAttachmentRequest));
            var response = new AddAttachmentResponse();
            response.Result = await _activeIncidentRepository.AddTaskAttachment(request.ActiveIncidentTaskId, request.AttachmentTitle, request.FileName, request.SourceFileName, request.FileSize, request.UserId, request.CompanyId, request.TimeZoneId);
            return response;
        }
    }
}
