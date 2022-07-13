using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType
{
    public class GetLibIncidentTypeHandler:IRequestHandler<GetLibIncidentTypeRequest, GetLibIncidentTypeResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetLibIncidentTypeHandler> _logger;
        public GetLibIncidentTypeHandler(IAdminQuery adminQuery, ILogger<GetLibIncidentTypeHandler> logger)
        {
            this._adminQuery = adminQuery;
            this._logger = logger;
        }

        public async Task<GetLibIncidentTypeResponse> Handle(GetLibIncidentTypeRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.GetLibIncidentType(request);
            return result;
        }
    }
}
