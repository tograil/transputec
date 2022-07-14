using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.AddLibIncidentType
{
    public class AddLibIncidentTypeHandler : IRequestHandler<AddLibIncidentTypeRequest, AddLibIncidentTypeResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<AddLibIncidentTypeHandler> _logger;
        
        public AddLibIncidentTypeHandler(IAdminQuery adminQuery, ILogger<AddLibIncidentTypeHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }
        public async Task<AddLibIncidentTypeResponse> Handle(AddLibIncidentTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.AddLibIncidentType(request);
            return result;
        }
    }
}
