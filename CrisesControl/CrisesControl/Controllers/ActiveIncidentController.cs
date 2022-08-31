using CrisesControl.Api.Application.Commands.ActiveIncidentTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AcceptTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddAttachment;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.AddNotes;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.CompleteTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DeclineTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.DelegateTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskAsset;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetActiveTaskCheckList;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetIncidentTasksAudit;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAssignedUsers;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskAudit;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskCheckListHistory;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskDetails;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetTaskUserList;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.GetUserTaskList;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.NewAdHocTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReallocateTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.ReassignTask;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveCheckListResponse;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SaveActiveTaskAsset;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.SendTaskUpdate;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.TakeOwnership;
using CrisesControl.Api.Application.Commands.ActiveIncidentTask.UnattendedTask;
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
    public class ActiveIncidentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IActiveIncidentQuery _activeIncidentQuery;

        public ActiveIncidentController(IMediator mediator, IActiveIncidentQuery activeIncidentQuery)
        {
            _mediator = mediator;
            _activeIncidentQuery = activeIncidentQuery;
        }
        /// <summary>
        /// Get list of user task
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserTask([FromRoute] GetUserTaskRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Active incident tasks
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("ActiveIncidentTasks/{ActiveIncidentID}")]
        public async Task<IActionResult> ActiveIncidentTasks([FromRoute] ActiveIncidentTasksRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
        [Route("AcceptTask/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> AcceptTask([FromRoute] AcceptTaskRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
        [Route("DeclineTask/{ActiveIncidentTaskID}/{TaskActionReason}")]
        public async Task<IActionResult> DeclineTask([FromRoute] DeclineTaskRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
        [Route("CompleteTask/{ActiveIncidentTaskID}/{TaskActionReason}/{TaskCompletionNote}/{SendUpdateTo}/{MessageMethod}/{CascadePlanID}")]
        public async Task<IActionResult> CompleteTask([FromRoute] CompleteTaskRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
        [Route("DelegateTask/{ActiveIncidentTaskID}/{TaskActionReason}/{TaskCompletionNote}/{SendUpdateTo}/{MessageMethod}/{CascadePlanID}")]
        public async Task<IActionResult> DelegateTask([FromRoute] DelegateTaskRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTaskAudit/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> GetTaskAudit([FromRoute] GetTaskAuditRequest request, CancellationToken cancellationToken)
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
        [Route("[action]/{ActiveCheckListId}")]
        public async Task<IActionResult> GetTaskCheckListHistory([FromRoute] GetTaskCheckListHistoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("[action]/{ActiveTaskID}")]
        public async Task<IActionResult> GetActiveTaskAsset([FromRoute] GetActiveTaskAssetRequest request, CancellationToken cancellationToken)
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("TakeOwnership/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> GetTaskAudit([FromRoute] TakeOwnershipRequest request, CancellationToken cancellationToken)
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
        [Route("GetTaskDetails/{ActiveIncidentTaskID}")]
        public async Task<IActionResult> GetTaskDetails([FromRoute] GetTaskDetailsRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
        [Route("AddNotes/{ActiveIncidentTaskID}/{TaskCompletionNote}")]
        public async Task<IActionResult> AddNotes([FromBody] AddNotesRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
        [Route("SaveActiveCheckListResponseRequest/{ActiveIncidentTaskID}/{CheckListResponse}")]
        public async Task<IActionResult> SaveActiveCheckListResponse([FromRoute] SaveActiveCheckListResponseRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
        [Route("UnattendedTask/{ActiveIncidentID}/{OutUserCompanyId}/{OutLoginUserId}")]
        public async Task<IActionResult> UnattendedTask([FromRoute] UnattendedTaskRequest request, CancellationToken cancellationToken)
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
        [HttpPost]
        [Route("NewAdHocTask/{ActiveIncidentID}/{TaskTitle}")]
        public async Task<IActionResult> NewAdHocTask([FromRoute] NewAdHocTaskRequest request, CancellationToken cancellationToken)
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
        [Route("GetUserTaskList/{ActiveIncidentId:int}/{CurrentUserId:int}/{CompanyId:int}")]
        public async Task<IActionResult> GetUserTaskList([FromRoute] GetUserTaskListRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.GetUserTaskList(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetTaskUser")]
        public async Task<IActionResult> GetTaskUserList([FromBody] GetTaskUserListRequest request, CancellationToken cancellationToken)
        {
            var result = await _activeIncidentQuery.GetTaskUserList(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetIncidentTasksAudit/{ActiveIncidentTaskID:int}/{CompanyId}")]
        public async Task<IActionResult> GetIncidentTasksAudit([FromRoute] GetIncidentTasksAuditRequest request)
        {
            var result = await _activeIncidentQuery.GetIncidentTasksAudit(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReassignTask")]
        public async Task<IActionResult> ReassignTask([FromBody] ReassignTaskRequest request, CancellationToken cancellationToken)
        {
            var result = _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddAttachment")]
        public async Task<IActionResult> AddAttachment([FromBody] AddAttachmentRequest request, CancellationToken cancellationToken)
        {
            var result = _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetActiveTaskCheckList/{ActiveIncidentTaskID:int}")]
        public async Task<IActionResult> GetActiveTaskCheckList([FromRoute] GetActiveTaskCheckListRequest request)
        {
            var result = await _activeIncidentQuery.GetActiveTaskCheckList(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetTaskCheckListHistory/{ActiveCheckListId:int}/{CompanyId:int}/{UserId:int}")]
        public async Task<IActionResult> GetTaskCheckListHistory([FromRoute] GetTaskCheckListHistoryRequest request)
        {
            var result = await _activeIncidentQuery.GetTaskCheckListHistory(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetActiveTaskAsset/{ActiveTaskId:int}/{CompanyId:int}/{UserId:int}")]
        public async Task<IActionResult> GetActiveTaskAsset([FromRoute] GetActiveTaskAssetRequest request)
        {
            var result = await _activeIncidentQuery.GetActiveTaskAsset(request);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveActiveTaskAsset")]
        public async Task<IActionResult> SaveActiveTaskAsset([FromBody] SaveActiveTaskAssetRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
