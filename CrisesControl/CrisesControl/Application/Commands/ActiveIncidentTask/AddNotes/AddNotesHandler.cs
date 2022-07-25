using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddNotes
{
    public class AddNotesHandler : IRequestHandler<AddNotesRequest, AddNotesResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<AddNotesHandler> _logger;
        public AddNotesHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<AddNotesHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
        }
        public async Task<AddNotesResponse> Handle(AddNotesRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.AddNotes(request);
            return result;
        }
    }
}
