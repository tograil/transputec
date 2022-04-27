using CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferences;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommunicationController : Controller {
        private readonly IMediator _mediator;

        public CommunicationController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("GetUserActiveConferences")]
        public async Task<IActionResult> GetUserActiveConferences([FromRoute] GetUserActiveConferencesRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
