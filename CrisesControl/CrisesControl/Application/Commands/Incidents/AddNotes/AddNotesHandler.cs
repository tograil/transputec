using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddNotes
{
    public class AddNotesHandler : IRequestHandler<AddNotesRequest, AddNotesResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<AddNotesHandler> _logger;
        public AddNotesHandler(ILogger<AddNotesHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<AddNotesResponse> Handle(AddNotesRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.AddNotes(request);
            return result;
        }
    }
}
