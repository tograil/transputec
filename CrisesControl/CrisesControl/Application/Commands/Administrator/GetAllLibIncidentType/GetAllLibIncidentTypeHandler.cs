using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncidentType
{
    public class GetAllLibIncidentTypeHandler : IRequestHandler<GetAllLibIncidentTypeRequest, GetAllLibIncidentTypeResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetAllLibIncidentTypeHandler> _logger;
        public GetAllLibIncidentTypeHandler(IAdminQuery adminQuery, ILogger<GetAllLibIncidentTypeHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }
        public async Task<GetAllLibIncidentTypeResponse> Handle(GetAllLibIncidentTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.GetAllLibIncidentType(request);
            return result;
        }
    }
}
