using Ardalis.GuardClauses;
using CrisesControl.Core.Incidents.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveTaskAsset
{
    public class SaveActiveTaskAssetHandler : IRequestHandler<SaveActiveTaskAssetRequest, SaveActiveTaskAssetResponse>
    {
        private readonly IActiveIncidentRepository _activeIncidentRepository;
        public SaveActiveTaskAssetHandler(IActiveIncidentRepository activeIncidentRepository)
        {
            _activeIncidentRepository = activeIncidentRepository;
        }

        public async Task<SaveActiveTaskAssetResponse> Handle(SaveActiveTaskAssetRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(SaveActiveTaskAssetRequest));
            var response = new SaveActiveTaskAssetResponse();
            response.Sucess = await _activeIncidentRepository.SaveActiveTaskAssets(request.ActiveTaskId,request.TaskAssets,request.CompanyId,request.UserId);
            return response;
        }
    }
}
