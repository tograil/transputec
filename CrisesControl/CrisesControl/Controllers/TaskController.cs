using CrisesControl.Api.Application.Commands.Tasks.AddNewIncidentTask;
using CrisesControl.Api.Application.Commands.Tasks.ChangePredecessor;
using CrisesControl.Api.Application.Commands.Tasks.CreateIncidentTaskHeader;
using CrisesControl.Api.Application.Commands.Tasks.DeleteTask;
using CrisesControl.Api.Application.Commands.Tasks.DeleteTaskAsset;
using CrisesControl.Api.Application.Commands.Tasks.ReorderTask;
using CrisesControl.Api.Application.Commands.Tasks.SaveTaskAsset;
using CrisesControl.Api.Application.Commands.Tasks.SaveTaskCheckList;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Application.Query.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : Controller
{
    private readonly IMediator _mediator;
    private readonly ITaskQuery _taskQuery;

    public TaskController(IMediator mediator,
        ITaskQuery taskQuery)
    {
        _mediator = mediator;
        _taskQuery = taskQuery;

    }

    [HttpGet]
    [Route("[action]/{incidentId}/{incidentTaskId}/{single}")]
    public async Task<IActionResult> GetIncidentTask([FromRoute] int incidentId, [FromRoute] int incidentTaskId, [FromRoute] bool single = false, CancellationToken cancellationToken = default)
    {
        var result = await _taskQuery.GetIncidentTask(incidentId, incidentTaskId, single, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{incidentId}/{incidentTaskId}/{taskHeaderId}")]
    public async Task<IActionResult> GetTaskDetail([FromRoute] int incidentId, [FromRoute] int incidentTaskId, [FromRoute] int taskHeaderId, CancellationToken cancellationToken = default)
    {
        var result = await _taskQuery.GetTaskDetail(incidentId, incidentTaskId, taskHeaderId, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{incidentTaskId}/{companyId}")]
    public IActionResult GetTaskAsset([FromRoute] int incidentTaskId, [FromRoute] int companyId)
    {
        var result = _taskQuery.GetTaskAsset(incidentTaskId, companyId);
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult GetChkResponseOptions()
    {
        var result = _taskQuery.GetChkResponseOptions();
        return Ok(result);
    }

    [HttpGet]
    [Route("[action]/{incidentTaskID}")]
    public IActionResult GetTaskCheckList([FromRoute] int incidentTaskId)
    {
        var result = _taskQuery.GetTaskCheckList(incidentTaskId);
        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CreateIncidentTaskHeader([FromBody] CreateIncidentTaskHeaderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> AddNewIncidentTask([FromBody] AddNewIncidentTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> ReorderTask([FromBody] ReorderTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> ChangePredecessor([FromBody] ChangePredecessorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveTaskAsset([FromBody] SaveTaskAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<IActionResult> DeleteTaskAsset([FromBody] DeleteTaskAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete]
    [Route("[action]")]
    public async Task<IActionResult> DeleteTask([FromBody] DeleteTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveTaskCheckList([FromBody] SaveTaskCheckListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CloneTask([FromBody] CloneTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

}