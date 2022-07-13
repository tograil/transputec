using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddLibIncident
{
    public class AddLibIncidentHandler : IRequestHandler<AddLibIncidentRequest, AddLibIncidentResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<AddLibIncidentHandler> _logger;
        
        public AddLibIncidentHandler(IAdminQuery adminQuery, ILogger<AddLibIncidentHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
            
        }
        public async Task<AddLibIncidentResponse> Handle(AddLibIncidentRequest request, CancellationToken cancellationToken)
        {
           
            var result = await _adminQuery.AddLibIncident(request);
            return result;
        }
    }
}
