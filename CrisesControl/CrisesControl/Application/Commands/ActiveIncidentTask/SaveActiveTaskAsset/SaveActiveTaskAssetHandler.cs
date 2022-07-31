using CrisesControl.Api.Application.Query;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveTaskAsset
{
    public class SaveActiveTaskAssetHandler : IRequestHandler<SaveActiveTaskAssetRequest, SaveActiveTaskAssetResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        private readonly ILogger<SaveActiveTaskAssetHandler> _logger;
        public SaveActiveTaskAssetHandler(IActiveIncidentQuery activeIncidentQuery, ILogger<SaveActiveTaskAssetHandler> logger)
        {
            this._activeIncidentQuery = activeIncidentQuery;
            this._logger = logger;
        }
        public async Task<SaveActiveTaskAssetResponse> Handle(SaveActiveTaskAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.SaveActiveTaskAsset(request);
            return result;
        }
    }
}
