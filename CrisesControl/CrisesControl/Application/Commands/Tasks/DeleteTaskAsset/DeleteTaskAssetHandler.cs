using Ardalis.GuardClauses;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.AssetAggregate;
using CrisesControl.Core.Companies.Repositories;
using CrisesControl.Core.Tasks.Repositories;
using MediatR;

namespace CrisesControl.Api.Application.Commands.Tasks.DeleteTaskAsset;

public class DeleteTaskAssetHandler
    : IRequestHandler<DeleteTaskAssetRequest, List<Assets>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly DateTimeOffset _requestDateTime = DateTimeOffset.Now;

    public DeleteTaskAssetHandler(ITaskRepository TaskRepository)
    {
        _taskRepository = TaskRepository;
    }

    public async Task<List<Assets>> Handle(DeleteTaskAssetRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(DeleteTaskAssetRequest));
        await _taskRepository.DeleteTaskAsset(request.IncidentTaskId, request.TaskAssets, request.CurrentUserId, request.CompanyId, cancellationToken);
        var assets = _taskRepository.GetTaskAsset(request.IncidentTaskId, request.CompanyId);
        return assets ?? new List<Assets>();
    }
}