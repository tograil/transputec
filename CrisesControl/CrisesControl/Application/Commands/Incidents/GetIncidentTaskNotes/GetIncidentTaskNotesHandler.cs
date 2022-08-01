using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetIncidentTaskNotes
{
    public class GetIncidentTaskNotesHandler : IRequestHandler<GetIncidentTaskNotesRequest, GetIncidentTaskNotesResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetIncidentTaskNotesHandler> _logger;
        public GetIncidentTaskNotesHandler( ILogger<GetIncidentTaskNotesHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetIncidentTaskNotesResponse> Handle(GetIncidentTaskNotesRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetIncidentTaskNotes(request);
            return result;
        }
    }
}
