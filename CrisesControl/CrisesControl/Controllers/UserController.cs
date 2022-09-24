using CrisesControl.Api.Application.Commands.Users.ActivateUser;
using CrisesControl.Api.Application.Commands.Users.DeleteUser;
using CrisesControl.Api.Application.Commands.Users.GetUser;
using CrisesControl.Api.Application.Commands.Users.Login;
using CrisesControl.Api.Application.Commands.Users.UpdateGroupMember;
using CrisesControl.Api.Application.Commands.Users.UpdateProfile;
using CrisesControl.Api.Application.Commands.Users.UpdateUser;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Application.Commands.Users.UpdateUserGroup;
using CrisesControl.Api.Application.Commands.Users.ValidateEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CrisesControl.Api.Application.Commands.Users.MembershipList;
using CrisesControl.Api.Application.Commands.Users.GetAllUsersDevice;
using CrisesControl.Api.Application.Commands.Users.GetUserComms;
using CrisesControl.Api.Application.Commands.Users.DeleteRegisteredUser;
using CrisesControl.Api.Application.Commands.Users.UpdateUserPhoto;
using CrisesControl.Api.Application.Commands.Users.UpdateUserPhone;
using CrisesControl.Api.Application.Commands.Users.CheckEmail;
using CrisesControl.Api.Application.Commands.Users.SendInvites;
using CrisesControl.Api.Application.Commands.Users.GetAllOneUserDeviceList;
using CrisesControl.Api.Application.Commands.Users.DeleteUserDevice;
using CrisesControl.Api.Application.Commands.Users.GetAllUser;
using CrisesControl.Api.Application.Commands.Users.AddUser;
using CrisesControl.Api.Application.Commands.Users.BulkAction;
using CrisesControl.Api.Application.Commands.Users.SendPasswordOTP;
using CrisesControl.Api.Application.Commands.Users.CheckUserLicense;
using CrisesControl.Api.Application.Commands.Users.GetUserCount;
using CrisesControl.Api.Application.Commands.Users.TrackUserDevice;
using CrisesControl.Api.Application.Commands.Users.GetUserMovements;
using CrisesControl.Api.Application.Commands.Users.GetUserGroups;
using CrisesControl.Api.Application.Commands.Users.OffDutySetting;
using CrisesControl.Api.Application.Commands.Users.GetUserDashboard;
using CrisesControl.Api.Application.Commands.Users.SaveDashboard;
using CrisesControl.Api.Application.Commands.Users.AddDashlet;
using CrisesControl.Api.Application.Commands.Users.GetUserSystemInfo;
using CrisesControl.Api.Application.Commands.Users.GetKeyHolders;
using CrisesControl.Api.Application.Commands.Users.ForgotPassword;
using CrisesControl.Api.Application.Commands.Users.LinkResetPassword;
using CrisesControl.Api.Application.Commands.Users.ResetPassword;
using CrisesControl.Api.Application.Commands.Users.ResetUserDeviceToken;
using CrisesControl.Api.Application.Commands.Users.UserRelation;
using CrisesControl.Api.Application.Commands.Users.GetUserId;

namespace CrisesControl.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
       

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route ("[action]")]
        public async Task<IActionResult> GetAllUser([FromQuery] GetAllUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetUser/{CompanyId:int}/{UserId:int}")]
        public async Task<IActionResult> GetUser([FromRoute] GetUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("ValidateLoginEmail")]
        public async Task<IActionResult> ValidateLoginEmail([FromQuery] ValidateEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetLoggedinUserInfo")]
        public async Task<IActionResult> GetLoggedinUserInfo([FromForm] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }


        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest UserModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UserModel, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("ReactivateUser")]
        public async Task<IActionResult> ReactivateUser([FromQuery] ActivateUserRequest activateUserRequest, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(activateUserRequest, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest UserModel, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UserModel, cancellationToken);
            return Ok(result);
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPut]
        [Route("UpdateUserGroup")]
        public async Task<IActionResult> UpdateUserGroup([FromBody] UpdateUserGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPut]
        [Route("UpdateGroupMember")]
        public async Task<IActionResult> UpdateGroupMember([FromBody] UpdateGroupMemberRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("MembershipList")]
        public async Task<IActionResult> MembershipList([FromQuery] MembershipRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("GetAllUserDeviceList")]
        public async Task<IActionResult> GetAllUserDeviceList([FromBody] GetAllUserDevicesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetUserComms")]
        public async Task<IActionResult> GetUserComms([FromBody] GetUserCommsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
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
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("DeleteUserDevice")]
        public async Task<IActionResult> DeleteUserDevice([FromBody] DeleteUserDeviceRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("UserRelations")]
        public async Task<IActionResult> UserRelations([FromRoute] UserRelationRequest request,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(Request, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("ResetUserDeviceToken/{UserId}")]
        public async Task<IActionResult> ResetUserDeviceToken([FromRoute] ResetUserDeviceTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request,  cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("BulkAction")]
        public async Task<ActionResult> BulkAction([FromBody] BulkActionRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request,  cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("SendPasswordOTP")]
        public async Task<ActionResult> SendPasswordOTP([FromBody] SendPasswordOTPRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request,cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("CheckUserLicense")]
        public async Task<ActionResult> CheckUserLicense([FromBody] CheckUserLicenseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetUserCount")]
        public async Task<ActionResult> GetUserCount([FromRoute] GetUserCountRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("TrackUserDevice")]
        public async Task<ActionResult> TrackUserDevice([FromRoute] TrackUserDeviceRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("GetUserMovements")]
        public async Task<ActionResult> GetUserMovements([FromRoute] GetUserMovementsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetUserGroups/{UserID}")]
        public async Task<ActionResult> GetUserGroups([FromRoute] GetUserGroupsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("OffDutySetting")]
        public async Task<ActionResult> OffDutySetting([FromBody] OffDutySettingRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("GetUserDashboard")]
        public async Task<ActionResult> GetUserDashboard([FromRoute]  GetUserDashboardRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        [HttpPost]
        [Route("SaveDashboard")]
        public async Task<ActionResult> SaveDashboard([FromBody] SaveDashboardRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("AddDashlet")]
        public async Task<ActionResult> AddDashlet([FromBody] AddDashletRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetUserId/{EmailAddress}")]
        public async Task<ActionResult> GetUserId([FromRoute] GetUserIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetUserSystemInfo")]
        public async Task<ActionResult> GetUserSystemInfo([FromRoute]  GetUserSystemInfoRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetKeyHolders([FromRoute] GetKeyHoldersRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Link Reset Password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LinkResetPassword([FromBody] LinkResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

    }
}
