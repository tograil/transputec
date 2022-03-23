using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserModel = CrisesControl.Core.Models.EmptyUser;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] GetUsersRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> GetUser([FromQuery] GetUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserModel UserModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UserModel, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel UserModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UserModel, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] UserModel UserModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UserModel, cancellationToken);
            return Ok(result);
        }
    }
}
