using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Incidents.AddIncidentAsset
{
    public class AddIncidentAssetHandler : IRequestHandler<AddIncidentAssetRequest, AddIncidentAssetResponse>
    {
        private readonly IIncidentQuery _incidentQuery;
        private readonly ILogger<AddIncidentAssetHandler> _logger;
        public AddIncidentAssetHandler(IIncidentQuery incidentQuery, ILogger<AddIncidentAssetHandler> logger)
        {
            this._incidentQuery = incidentQuery;
            this._logger = logger;
        }
        public Task<AddIncidentAssetResponse> Handle(AddIncidentAssetRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
