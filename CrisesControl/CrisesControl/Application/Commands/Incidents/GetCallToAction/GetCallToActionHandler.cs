using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.GetCallToAction
{
    public class GetCallToActionHandler : IRequestHandler<GetCallToActionRequest, GetCallToActionResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<GetCallToActionHandler> _logger;
        public GetCallToActionHandler(ILogger<GetCallToActionHandler> logger, IIncidentQuery incidentQuery)
        {
            _logger = logger;
            _incidentQuery = incidentQuery;
        }
        public async Task<GetCallToActionResponse> Handle(GetCallToActionRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.GetCallToAction(request);
            return result;
        }
    }
}
