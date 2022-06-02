using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident
{
    public class GetAllLibIncidentHandler : IRequestHandler<GetAllLibIncidentRequest, GetAllLibIncidentResponse>
    {
        private readonly IAdminQuery _adminQuery;
        private readonly ILogger<GetAllLibIncidentHandler> _logger;
        public GetAllLibIncidentHandler(IAdminQuery adminQuery, ILogger<GetAllLibIncidentHandler> logger)
        {
          this._adminQuery = adminQuery;
           this._logger = logger;
        }
        public async Task<GetAllLibIncidentResponse> Handle(GetAllLibIncidentRequest request, CancellationToken cancellationToken)
        {
            var result = await _adminQuery.GetAllLibIncident(request);
            return result;
        }
    }
}
