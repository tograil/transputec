using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetAttachments
{
    public class GetAttachmentsHandler : IRequestHandler<GetAttachmentsRequest, GetAttachmentsResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetAttachmentsHandler> _logger;
        public GetAttachmentsHandler(ILogger<GetAttachmentsHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetAttachmentsResponse> Handle(GetAttachmentsRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetAttachments(request);
            return result;
        }
    }
}
