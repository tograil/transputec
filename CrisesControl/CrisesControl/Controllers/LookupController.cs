using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpDept;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpLoc;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpUser;
using CrisesControl.Api.Application.Commands.Lookup.GetIcons;
using CrisesControl.Api.Application.Commands.Lookup.GetImportTemplates;
using CrisesControl.Api.Application.Commands.Lookup.GetTempDept;
using CrisesControl.Api.Application.Commands.Lookup.GetTempLoc;
using CrisesControl.Api.Application.Commands.Lookup.GetTempUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[Action]")]
    [Authorize]
    public class LookupController : ControllerBase
    {
        private readonly IMediator _mediator;
        public LookupController(IMediator mediator)
        {
            this._mediator = mediator;
        }


        [HttpGet]
        public async Task<IActionResult> GetTimeZone()
        {
            try
            {


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCountry()
        {
            try
            {

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("[action]/{Type}")]
        public async Task<IActionResult> GetImportTemplates([FromRoute] GetImportTemplatesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetIcons([FromRoute] GetIconsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllTmpUser([FromRoute] GetAllTmpUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllTmpLoc([FromRoute] GetAllTmpLocRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllTmpDept([FromRoute] GetAllTmpDeptRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{TempUserId}")]
        public async Task<IActionResult> GetTempUser([FromRoute] GetTempUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{TempLocationId}")]
        public async Task<IActionResult> GetTempLoc([FromRoute] GetTempLocRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{TempDeptId}")]
        public async Task<IActionResult> GetTempDept([FromRoute] GetTempDeptRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

    }
}