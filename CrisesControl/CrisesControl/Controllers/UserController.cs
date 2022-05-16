using CrisesControl.Api.Application.Commands.Users.CreateUser;
using CrisesControl.Api.Application.Commands.Users.DeleteUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.UpdateUser;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IUserQuery _userQuery;

        public UserController(IMediator mediator, IUserQuery userQuery)
        {
            _mediator = mediator;
            _userQuery = userQuery;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Index([FromForm] GetUsersRequest request, CancellationToken cancellationToken)
        {
            var result = await _userQuery.GetUsers(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("{CompanyId:int}/{UserId:int}")]
        public async Task<IActionResult> GetUser([FromRoute] GetUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _userQuery.GetUser(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetLoggedinUserInfo([FromQuery] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _userQuery.GetLoggedInUserInfo(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest UserModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UserModel, cancellationToken);
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
