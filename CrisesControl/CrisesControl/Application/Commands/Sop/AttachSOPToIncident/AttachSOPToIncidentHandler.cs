using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Sop.AttachSOPToIncident
{
    public class AttachSOPToIncidentHandler : IRequestHandler<AttachSOPToIncidentRequest, AttachSOPToIncidentResponse>
    {
        private readonly ISopQuery _sopQuery;
        private readonly ILogger<AttachSOPToIncidentHandler> _logger;
        public AttachSOPToIncidentHandler(ISopQuery sopQuery, ILogger<AttachSOPToIncidentHandler> logger)
        {
            this._sopQuery = sopQuery;
            this._logger = logger;
        }
        public async Task<AttachSOPToIncidentResponse> Handle(AttachSOPToIncidentRequest request, CancellationToken cancellationToken)
        {
            var result = await _sopQuery.AttachSOPToIncident(request);
            return result;
        }
    }
}
