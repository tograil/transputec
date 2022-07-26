using CrisesControl.Api.Application.Commands.Academy.GetToursSteps;
using CrisesControl.Api.Application.Commands.Academy.GetUserVideos;
using CrisesControl.Api.Application.Commands.Academy.GetVideos;
using CrisesControl.Api.Application.Commands.Academy.SaveTourLog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AcademyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AcademyController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpGet]
        [Route("GetVideos")]
        public async Task<IActionResult> GetVideos([FromRoute] GetVideosRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetUserVideos")]
        public async Task<IActionResult> GetUserVideos([FromRoute] GetUserVideosRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetToursSteps")]
        public async Task<IActionResult> GetToursSteps([FromRoute] GetToursStepsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("SaveTourLog")]
        public async Task<IActionResult> SaveTourLog([FromBody] SaveTourLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
