using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.UpdateSegregationLink
{
    public class UpdateSegregationLinkHandler : IRequestHandler<UpdateSegregationLinkRequest, UpdateSegregationLinkResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<UpdateSegregationLinkHandler> _logger;
        public UpdateSegregationLinkHandler(IIncidentQuery incidentQuery, ILogger<UpdateSegregationLinkHandler> logger)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
        }
        public async Task<UpdateSegregationLinkResponse> Handle(UpdateSegregationLinkRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.UpdateSegregationLink(request);
            return result;
        }
    }
}
