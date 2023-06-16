using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.SaveIncidentParticipants
{
    public class SaveIncidentParticipantsHandler : IRequestHandler<SaveIncidentParticipantsRequest, SaveIncidentParticipantsResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<SaveIncidentParticipantsHandler> _logger;
        //private readonly GetIncidentActionValidator _getIncidentActionValidator;
        public SaveIncidentParticipantsHandler(IIncidentQuery incidentQuery, ILogger<SaveIncidentParticipantsHandler> logger)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            //this._getIncidentActionValidator = getIncidentActionValidator;
        }
        public async Task<SaveIncidentParticipantsResponse> Handle(SaveIncidentParticipantsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveIncidentParticipantsRequest));
            var result = await _incidentQuery.SaveIncidentParticipants(request);
            return result;
        }
    }
}
