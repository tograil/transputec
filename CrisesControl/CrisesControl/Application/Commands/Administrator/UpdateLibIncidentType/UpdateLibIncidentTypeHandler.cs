using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncidentType
{
    public class UpdateLibIncidentTypeHandler : IRequestHandler<UpdateLibIncidentTypeRequest, UpdateLibIncidentTypeResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<UpdateLibIncidentTypeHandler> _logger;
        public UpdateLibIncidentTypeHandler(IAdminQuery adminQuery, ILogger<UpdateLibIncidentTypeHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }
        public async Task<UpdateLibIncidentTypeResponse> Handle(UpdateLibIncidentTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.UpdateLibIncidentType(request);
            return result;
        }
    }
}
