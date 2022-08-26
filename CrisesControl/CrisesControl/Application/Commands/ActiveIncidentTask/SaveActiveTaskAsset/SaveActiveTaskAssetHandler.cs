using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Query;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveTaskAsset
{
    public class SaveActiveTaskAssetHandler : IRequestHandler<SaveActiveTaskAssetRequest, SaveActiveTaskAssetResponse>
    {
        private readonly IActiveIncidentQuery _activeIncidentQuery;
        public SaveActiveTaskAssetHandler(IActiveIncidentQuery activeIncidentQuery)
        {
            _activeIncidentQuery = activeIncidentQuery;
        }

        public async Task<SaveActiveTaskAssetResponse> Handle(SaveActiveTaskAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveActiveTaskAssetRequest));
            var response = new SaveActiveTaskAssetResponse();
            response = await _activeIncidentQuery.SaveActiveTaskAsset(request);
            return response;
        }
    }
}
