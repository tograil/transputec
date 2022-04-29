using CrisesControl.Api.Application.Commands.ExTriggers.GetAllExTrigger;
using CrisesControl.Api.Application.Commands.ExTriggers.GetExTrigger;
using CrisesControl.Api.Application.Commands.ExTriggers.GetImpTrigger;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExTriggerController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IExTriggerQuery _exTriggerQuery;
        public ExTriggerController(IMediator mediator, IExTriggerQuery exTriggerQuery)
        {
            this._mediator = mediator;
            this._exTriggerQuery = exTriggerQuery;
        }
        [HttpGet("GetAllExTrigger")]
        public async Task<IActionResult> GetAllExTrigger([FromQuery] GetAllExTriggerRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet("GetExTrigger")]
        public async Task<IActionResult> GetExTrigger([FromQuery] GetExTriggerRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet("GetImpTrigger")]
        public async Task<IActionResult> GetImpTrigger([FromQuery] GetImpTriggerRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }


    }
}
