using Ardalis.GuardClauses;
using CrisesControl.Core.Tasks.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.SaveTaskAsset;

public class SaveTaskAssetHandler
    : IRequestHandler<SaveTaskAssetRequest, List<CrisesControl.Core.Assets.Assets>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly string _timeZoneId = "GMT Standard Time";

    public SaveTaskAssetHandler(ITaskRepository TaskRepository)
    {
        _taskRepository = TaskRepository;
    }

    public async Task<List<CrisesControl.Core.Assets.Assets>> Handle(SaveTaskAssetRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(SaveTaskAssetRequest));
        await _taskRepository.SaveTaskAssets(request.IncidentTaskId, request.TaskAssets, request.CurrentUserId, request.CompanyId, _timeZoneId, cancellationToken);
        var assets = _taskRepository.GetTaskAsset(request.IncidentTaskId, request.CompanyId);
        return assets ?? new List<CrisesControl.Core.Assets.Assets>();
    }
}