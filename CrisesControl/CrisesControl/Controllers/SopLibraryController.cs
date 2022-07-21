using CrisesControl.Api.Application.Commands.SopLibrary.DeleteSOPLib;
using CrisesControl.Api.Application.Commands.SopLibrary.GetSopSection;
using CrisesControl.Api.Application.Commands.SopLibrary.GetSopSections;
using CrisesControl.Api.Application.Commands.SopLibrary.SaveLibSection;
using CrisesControl.Api.Application.Commands.SopLibrary.UseLibraryText;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SopLibraryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SopLibraryController(IMediator mediator)
        {
            this._mediator = mediator;
        }
        [HttpGet]
        [Route("GetSopSections")]
        public async Task<IActionResult> GetSopSections([FromRoute] GetSopSectionsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("SaveLibSection")]
        public async Task<IActionResult> SaveLibSection([FromBody] SaveLibSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetSopSection/{SOPHeaderID}")]
        public async Task<IActionResult> GetSopSection([FromRoute] GetSopSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("UseLibraryText/{SOPHeaderID}")]
        public async Task<IActionResult> UseLibraryText([FromRoute] UseLibraryTextRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpDelete]
        [Route("DeleteSOPLib")]
        public async Task<IActionResult> DeleteSOPLib([FromRoute] DeleteSOPLibRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
