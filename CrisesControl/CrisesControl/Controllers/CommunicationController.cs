using CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferenceList;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommunicationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICommunicationQuery _communicationQuery;

        public CommunicationController(IMediator mediator, ICommunicationQuery communicationQuery)
        {
            this._mediator = mediator;
            this._communicationQuery=communicationQuery;
        }
        [HttpGet]
        public async Task<IActionResult> GetUserActiveConferrenceList([FromQuery] GetUserActiveConferenceListRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
