using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskAsset
{
    public class GetActiveTaskAssetHandler : IRequestHandler<GetActiveTaskAssetRequest, GetActiveTaskAssetResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<GetActiveTaskAssetHandler> _logger;

        public GetActiveTaskAssetHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<GetActiveTaskAssetHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;

        }
        public async Task<GetActiveTaskAssetResponse> Handle(GetActiveTaskAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.GetActiveTaskAsset(request);
            return result;
        }
    }
}
