using CrisesControl.Api.Application.Commands.Scheduler.GetAllJobs;
using CrisesControl.Api.Application.Commands.Scheduler.GetJob;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class SchedulerController : Controller
{
    private readonly IMediator _mediator;
    private readonly ISchedulerQuery _schedulerQuery;
    public SchedulerController(ISchedulerQuery schedulerQuery, IMediator mediator)
    {
        this._schedulerQuery = schedulerQuery;
        this._mediator = mediator;
    }
    [HttpGet]
    [Route("GetAllJobs")]
    public async Task<IActionResult> GetAllJobs([FromRoute] GetAllJobsRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
    [HttpGet]
    [Route("GetJob/{CompanyID:int}/{JobId:int}")]
    public async Task<IActionResult> GetJob([FromRoute] GetJobRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);
        return Ok(result);
    }
}

