using CrisesControl.Api.Application.Commands.Scheduler.AddJob;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class SchedulerController : Controller
{
    private readonly IMediator _mediator;

    public SchedulerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> AddJob([FromBody] AddJobRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }
}