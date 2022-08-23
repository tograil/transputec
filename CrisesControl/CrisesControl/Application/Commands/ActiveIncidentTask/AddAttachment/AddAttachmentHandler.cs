using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddAttachment
{
    public class AddAttachmentHandler : IRequestHandler<AddAttachmentRequest, AddAttachmentResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<AddAttachmentHandler> _logger;
        public AddAttachmentHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<AddAttachmentHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
        }
        public Task<AddAttachmentResponse> Handle(AddAttachmentRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
