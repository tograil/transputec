using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers {
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class MessagingController : Controller {
        private readonly IMediator _mediator;

        public MessagingController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("{CurrentUserId:int}/NotificationCount")]
        public async Task<IActionResult> GetNotificationsCount([FromRoute] GetNotificationsCountRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
    }
}
