using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.SegregationLinks
{
    public class SegregationLinksHandler : IRequestHandler<SegregationLinksRequest, SegregationLinksResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<SegregationLinksHandler> _logger;
        
        public SegregationLinksHandler(IIncidentQuery incidentQuery, ILogger<SegregationLinksHandler> logger)
        {

            this._logger = logger;
            this._incidentQuery = incidentQuery;
            
        }
        public async Task<SegregationLinksResponse> Handle(SegregationLinksRequest request, CancellationToken cancellationToken)
        {
            var result = await _incidentQuery.SegregationLinks(request);
            return result;
        }
    }
}
