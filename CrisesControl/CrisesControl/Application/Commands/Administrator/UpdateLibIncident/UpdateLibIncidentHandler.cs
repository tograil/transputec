using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncident
{
    public class UpdateLibIncidentHandler : IRequestHandler<UpdateLibIncidentRequest, UpdateLibIncidentResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<UpdateLibIncidentHandler> _logger;
    
        public UpdateLibIncidentHandler(IAdminQuery adminQuery, ILogger<UpdateLibIncidentHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
           
        }
        public async Task<UpdateLibIncidentResponse> Handle(UpdateLibIncidentRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.UpdateLibIncident(request);
            return result;
        }
    }
}
