using CrisesControl.Api.Application.Commands.Tasks.AddNewIncidentTask;
using CrisesControl.Api.Application.Commands.Tasks.ChangePredecessor;
using CrisesControl.Api.Application.Commands.Tasks.CreateIncidentTaskHeader;
using CrisesControl.Api.Application.Commands.Tasks.DeleteTask;
using CrisesControl.Api.Application.Commands.Tasks.DeleteTaskAsset;
using CrisesControl.Api.Application.Commands.Tasks.ReorderTask;
using CrisesControl.Api.Application.Commands.Tasks.SaveTaskAsset;
using CrisesControl.Api.Application.Commands.Tasks.SaveTaskCheckList;
using CrisesControl.Api.Application.Query;
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

    /// <summary>
    /// Get Tasks of an incident.
    /// </summary>
    /// <param name="incidentId"></param>
    /// <param name="incidentTaskId"></param>
    /// <param name="single">Set false to retrieve all tasks for incident, set true to retrieve task with the particular incidentTaskId.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{incidentId}/{incidentTaskId}/{single}")]
    public async Task<IActionResult> GetIncidentTask([FromRoute] int incidentId, [FromRoute] int incidentTaskId, [FromRoute] bool single = false, CancellationToken cancellationToken = default)
    {
        var result = await _taskQuery.GetIncidentTask(incidentId, incidentTaskId, single, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get Task by ID.
    /// </summary>
    /// <param name="incidentId"></param>
    /// <param name="incidentTaskId"></param>
    /// <param name="taskHeaderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{incidentId}/{incidentTaskId}/{taskHeaderId}")]
    public async Task<IActionResult> GetTaskDetail([FromRoute] int incidentId, [FromRoute] int incidentTaskId, [FromRoute] int taskHeaderId, CancellationToken cancellationToken = default)
    {
        var result = await _taskQuery.GetTaskDetail(incidentId, incidentTaskId, taskHeaderId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get Task Asset of a Task.
    /// </summary>
    /// <param name="incidentTaskId"></param>
    /// <param name="companyId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{incidentTaskId}/{companyId}")]
    public IActionResult GetTaskAsset([FromRoute] int incidentTaskId, [FromRoute] int companyId)
    {
        var result = _taskQuery.GetTaskAsset(incidentTaskId, companyId);
        return Ok(result);
    }

    /// <summary>
    /// Get all Checklist Reponse Options.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]")]
    public IActionResult GetChkResponseOptions()
    {
        var result = _taskQuery.GetChkResponseOptions();
        return Ok(result);
    }

    /// <summary>
    /// Get checklist for a task.
    /// </summary>
    /// <param name="incidentTaskId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("[action]/{incidentTaskId}")]
    public IActionResult GetTaskCheckList([FromRoute] int incidentTaskId)
    {
        var result = _taskQuery.GetTaskCheckList(incidentTaskId);
        return Ok(result);
    }

    /// <summary>
    /// Create a task header for an incident.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CreateIncidentTaskHeader([FromBody] CreateIncidentTaskHeaderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Create a new task for an incident.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> AddNewIncidentTask([FromBody] AddNewIncidentTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Change the order of a tasks in an incident.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> ReorderTask([FromBody] ReorderTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Change predecessor(s) of a task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("[action]")]
    public async Task<IActionResult> ChangePredecessor([FromBody] ChangePredecessorRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Save task asset(s) for a task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveTaskAsset([FromBody] SaveTaskAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete task asset(s) of a task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("[action]")]
    public async Task<IActionResult> DeleteTaskAsset([FromBody] DeleteTaskAssetRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete a task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("[action]")]
    public async Task<IActionResult> DeleteTask([FromBody] DeleteTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Save checklist for a task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> SaveTaskCheckList([FromBody] SaveTaskCheckListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Clone a task.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CloneTask([FromBody] CloneTaskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }

}