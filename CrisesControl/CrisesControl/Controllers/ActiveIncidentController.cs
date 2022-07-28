using CrisesControl.Api.Application.Commands.ActiveIncidentTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddNotes;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DelegateTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAssignedUsers;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAudit;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskDetails;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.NewAdHocTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReallocateTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveCheckListResponse;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SendTaskUpdate;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.TakeOwnership;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.UnattendedTask;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActiveIncidentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActiveIncidentController(IMediator mediator)
        {
            _mediator = mediator;
        }
       
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserTask([FromRoute] GetUserTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("ActiveIncidentTasks/{ActiveIncidentID}")]
        public async Task<IActionResult> ActiveIncidentTasks([FromRoute] ActiveIncidentTasksRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("AcceptTask/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> AcceptTask([FromRoute] AcceptTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("DeclineTask/{ActiveIncidentTaskID}/{TaskActionReason}")]
        public async Task<IActionResult> DeclineTask([FromRoute] DeclineTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("CompleteTask/{ActiveIncidentTaskID}/{TaskActionReason}/{TaskCompletionNote}/{SendUpdateTo}/{MessageMethod}/{CascadePlanID}")]
        public async Task<IActionResult> CompleteTask([FromRoute] CompleteTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("DelegateTask/{ActiveIncidentTaskID}/{TaskActionReason}/{TaskCompletionNote}/{SendUpdateTo}/{MessageMethod}/{CascadePlanID}")]
        public async Task<IActionResult> DelegateTask([FromRoute] DelegateTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("ReallocateTask/{ActiveIncidentTaskID}/{TaskActionReason}/{TaskCompletionNote}/{SendUpdateTo}/{MessageMethod}/{CascadePlanID}")]
        public async Task<IActionResult> ReallocateTask([FromRoute] ReallocateTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetTaskAssignedUsers/{ActiveIncidentTaskID}/{TypeName}")]
        public async Task<IActionResult> GetTaskAssignedUsers([FromRoute] GetTaskAssignedUsersRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetTaskAudit/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> GetTaskAudit([FromRoute] GetTaskAuditRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("SendTaskUpdate/{ActiveIncidentTaskID}/{TaskActionReason}/{SendUpdateTo}/{MessageMethod}")]
        public async Task<IActionResult> SendTaskUpdate([FromRoute] SendTaskUpdateRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("TakeOwnership/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> GetTaskAudit([FromRoute] TakeOwnershipRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetTaskDetails/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> GetTaskDetails([FromRoute] GetTaskDetailsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("AddNotes/{ActiveIncidentTaskID}/{TaskCompletionNote}")]
        public async Task<IActionResult> AddNotes([FromBody] AddNotesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        //[HttpPost]
        //[Route("SaveActiveCheckListResponseRequest/{ActiveIncidentTaskID}/{CheckListResponse}")]
        //public async Task<IActionResult> SaveActiveCheckListResponse([FromRoute] SaveActiveCheckListResponseRequest request, CancellationToken cancellationToken)
        //{
        //    var result = await _mediator.Send(request, cancellationToken);
        //    return Ok(result);
        //}
        [HttpPost]
        [Route("UnattendedTask/{ActiveIncidentID}/{OutUserCompanyId}/{OutLoginUserId}")]
        public async Task<IActionResult> UnattendedTask([FromRoute] UnattendedTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpPost]
        [Route("NewAdHocTask/{ActiveIncidentID}/{TaskTitle}")]
        public async Task<IActionResult> NewAdHocTask([FromRoute] NewAdHocTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
