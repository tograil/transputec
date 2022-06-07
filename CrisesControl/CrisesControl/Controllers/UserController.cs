using CrisesControl.Api.Application.Commands.Users.ActivateUser;
using CrisesControl.Api.Application.Commands.Users.CreateUser;
using CrisesControl.Api.Application.Commands.Users.DeleteUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.GetUsers;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.UpdateGroupMember;
using CrisesControl.Api.Application.Commands.Users.UpdateProfile;
using CrisesControl.Api.Application.Commands.Users.UpdateUser;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Application.Commands.Users.UpdateUserGroup;
using CrisesControl.Api.Application.Commands.Users.ValidateEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserModel = CrisesControl.Core.Models.EmptyUser;
using CrisesControl.Api.Application.Commands.Users.MembershipList;
using CrisesControl.Api.Application.Commands.Users.GetAllUsersDevice;
using CrisesControl.Core.Users;
using CrisesControl.Api.Application.Commands.Users.GetUserComms;
using CrisesControl.Api.Application.Commands.Users.DeleteRegisteredUser;
using CrisesControl.Api.Application.Commands.Users.UpdateUserPhoto;
using CrisesControl.Api.Application.Commands.Users.UpdateUserPhone;
using CrisesControl.Api.Application.Commands.Users.CheckEmail;
using CrisesControl.Api.Application.Commands.Users.SendInvites;
using CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList;
using CrisesControl.Api.Application.Commands.Users.DeleteUserDevice;

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
        [HttpGet("ValidateLoginEmail")]
        public async Task<IActionResult> ValidateLoginEmail([FromQuery] ValidateEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("getUserInfo")]
        public async Task<IActionResult> GetLoggedinUserInfo([FromForm] LoginRequest request, CancellationToken cancellationToken)
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
        [HttpPut ("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut("UpdateUserGroup")]
        public async Task<IActionResult> UpdateUserGroup([FromBody] UpdateUserGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut("UpdateGroupMember")]
        public async Task<IActionResult> UpdateGroupMember([FromBody] UpdateGroupMemberRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet("MembershipList")]
        public async Task<IActionResult> MembershipList([FromQuery] MembershipRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost("GetAllUserDeviceList")]
        public async Task<IActionResult> GetAllUserDeviceList([FromBody] GetAllUserDevicesRequest request, CancellationToken cancellationToken)
        {
            var result = await _userQuery.GetAllUserDeviceList(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUserComms")]
        public async Task<IActionResult> GetUserComms([FromBody] GetUserCommsRequest request, CancellationToken cancellationToken)
        {
            var result = await _userQuery.GetUserComms(request, cancellationToken);
            return Ok(result);
        }


        [HttpPost]
        [Route("DeleteRegisteredUser")]
        public async Task<IActionResult> DeleteRegisteredUser([FromBody] DeleteRegisteredUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateUserPhoto")]
        public async Task<IActionResult> UpdateUserPhoto([FromBody] UpdateUserPhotoRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateUserPhone")]
        public async Task<IActionResult> UpdateUserPhone([FromBody] UpdateUserPhoneRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("CheckEmail")]
        public async Task<IActionResult> CheckMail([FromBody] CheckEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("SendInvites")]
        public async Task<IActionResult> SendInvites([FromBody] SendInvitesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetAllOneUserDeviceList")]
        public async Task<IActionResult> GetAllOneUserDeviceList([FromBody] GetAllOneUserDeviceListRequest request, CancellationToken cancellationToken)
        {
            var result = await _userQuery.GetAllOneUserDeviceList(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("DeleteUserDevice")]
        public async Task<IActionResult> DeleteUserDevice([FromBody] DeleteUserDeviceRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
