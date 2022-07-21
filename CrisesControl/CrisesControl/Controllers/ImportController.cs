using CrisesControl.Api.Application.Commands.Import.DownloadImportResult;
using CrisesControl.Api.Application.Commands.Import.GetValidationResult;
using CrisesControl.Api.Application.Commands.Import.GroupOnlyImport;
using CrisesControl.Api.Application.Commands.Import.GroupOnlyUpload;
using CrisesControl.Api.Application.Commands.Import.LocationOnlyImport;
using CrisesControl.Api.Application.Commands.Import.LocationOnlyUpload;
using CrisesControl.Api.Application.Commands.Import.ProcessUserImport;
using CrisesControl.Api.Application.Commands.Import.QueueImportJob;
using CrisesControl.Api.Application.Commands.Import.UserCompleteImport;
using CrisesControl.Api.Application.Commands.Import.UserCompleteUpload;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ImportController : Controller
    {
        private readonly IMediator _mediator;

        [HttpGet("DownloadImportResult")]
        public async Task<IActionResult> DownloadImportResult([FromQuery] DownloadImportResultRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetValidationResult")]
        public async Task<IActionResult> GetValidationResult([FromBody] GetValidationResultRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GroupOnlyImport")]
        public async Task<IActionResult> GroupOnlyImport([FromBody] GroupOnlyImportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GroupOnlyUpload")]
        public async Task<IActionResult> GroupOnlyUpload([FromBody] GroupOnlyUploadRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("LocationOnlyImport")]
        public async Task<IActionResult> LocationOnlyImport([FromBody] LocationOnlyImportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("LocationOnlyUpload")]
        public async Task<IActionResult> LocationOnlyUpload([FromBody] LocationOnlyUploadRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("ProcessUserImport")]
        public async Task<IActionResult> ProcessUserImport([FromBody] ProcessUserImportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("QueueImportJob")]
        public async Task<IActionResult> QueueImportJob([FromBody] QueueImportJobRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("UserCompleteImport")]
        public async Task<IActionResult> UserCompleteImport([FromBody] UserCompleteImportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("UserCompleteUpload")]
        public async Task<IActionResult> UserCompleteUpload([FromBody] UserCompleteUploadRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
