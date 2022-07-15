using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActiveIncidentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActiveIncidentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserTask([FromRoute] GetUserTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("ActiveIncidentTasks/{ActiveIncidentID}")]
        public async Task<IActionResult> ActiveIncidentTasks([FromRoute] ActiveIncidentTasksRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("AcceptTask/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> AcceptTask([FromRoute] AcceptTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("DeclineTask/{ActiveIncidentTaskID}/{TaskActionReason}")]
        public async Task<IActionResult> DeclineTask([FromRoute] DeclineTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
