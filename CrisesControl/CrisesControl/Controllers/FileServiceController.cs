using CrisesControl.Api.Application.Commands.FileService.UploadToAzure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileServiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FileServiceController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost]
        [Route("UploadToAzure")]
        public async Task<IActionResult> UploadToAzure([FromRoute] UploadToAzureRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
