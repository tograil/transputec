using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.SaveIncidentJob
{
    public class SaveIncidentJobHandler : IRequestHandler<SaveIncidentJobRequest, SaveIncidentJobResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<SaveIncidentJobHandler> _logger;
       
        public SaveIncidentJobHandler(IIncidentQuery incidentQuery, ILogger<SaveIncidentJobHandler> logger)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            
        }
        public async Task<SaveIncidentJobResponse> Handle(SaveIncidentJobRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.SaveIncidentJob(request);
            return result;
        }
    }
}
