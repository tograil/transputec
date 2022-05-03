using CrisesControl.Api.Application.Commands.Users.ActivateUser;
using CrisesControl.Api.Application.Commands.Users.CreateUser;
using CrisesControl.Api.Application.Commands.Users.DeleteUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.UpdateUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserModel = CrisesControl.Core.Models.EmptyUser;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [AllowAnonymous]
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
        [HttpGet("getlogininfo")]
        public async Task<IActionResult> GetLoggedinUserInfo([FromQuery] LoginRequest request, CancellationToken cancellationToken)
        {
            //var userId = this.User.FindFirstValue("sub");
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest UserModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UserModel, cancellationToken);
            return Ok(result);
        }

        [HttpGet("ReactivateUser")]
        public async Task<IActionResult> ReactivateUser([FromQuery] ActivateUserRequest activateUserRequest, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(activateUserRequest, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest UserModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UserModel, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
