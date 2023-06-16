using CrisesControl.Api.Application.Commands.Sop.AttachSOPToIncident;
using CrisesControl.Api.Application.Commands.Sop.DeleteSOP;
using CrisesControl.Api.Application.Commands.Sop.GetCompanySOP;
using CrisesControl.Api.Application.Commands.Sop.GetSopSection;
using CrisesControl.Api.Application.Commands.Sop.GetSOPSectionLibrary;
using CrisesControl.Api.Application.Commands.Sop.GetSopSections;
using CrisesControl.Api.Application.Commands.Sop.GetTagList;
using CrisesControl.Api.Application.Commands.Sop.LibraryTextModel;
using CrisesControl.Api.Application.Commands.Sop.RemoveSection;
using CrisesControl.Api.Application.Commands.Sop.ReorderSection;
using CrisesControl.Api.Application.Commands.Sop.SaveSOPHeader;
using CrisesControl.Api.Application.Commands.Sop.SaveSopSection;
using CrisesControl.Api.Application.Commands.Sop.UpdateSOPAsset;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SopController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SopController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCompanySOP([FromRoute] GetCompanySOPRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SaveSOPHeader([FromRoute] SaveSOPHeaderRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetTagList([FromRoute] GetTagListRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{SOPHeaderID}")]
        public async Task<IActionResult> GetSopSections([FromRoute] GetSopSectionsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> SaveSopSection([FromRoute] SaveSopSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpDelete]
        [Route("[action]/{ContentSectionID}")]
        public async Task<IActionResult> RemoveSection([FromRoute] RemoveSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{SOPHeaderID}/{ContentSectionID}")]
        public async Task<IActionResult> GetSopSection([FromRoute] GetSopSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/SectionOrder")]
        public async Task<IActionResult> ReorderSection([FromRoute] ReorderSectionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSOPSectionLibrary([FromRoute] GetSOPSectionLibraryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("[action]/{SOPHeaderID}/{SOPFileName}")]
        public async Task<IActionResult> AttachSOPToIncident([FromRoute] AttachSOPToIncidentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("[action]/{IncidentName}/{IncidentType}/{Sector}/{SectionTitle}")]
        public async Task<IActionResult> LibraryTextModel([FromRoute] LibraryTextModelRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("[action]/{ISOPHeaderID}")]
        public async Task<IActionResult> DeleteSOP([FromRoute] DeleteSOPRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("[action]/{SOPHeaderID}/{AssetID}")]
        public async Task<IActionResult> UpdateSOPAsset([FromRoute] UpdateSOPAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
