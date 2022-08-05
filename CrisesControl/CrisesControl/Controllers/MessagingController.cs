using CrisesControl.Api.Application.Commands.Messaging.GetAttachment;
using CrisesControl.Api.Application.Commands.Messaging.GetConfRecordings;
using CrisesControl.Api.Application.Commands.Messaging.GetConfUser;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageAttachment;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageDetails;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageGroupList;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses;
using CrisesControl.Api.Application.Commands.Messaging.GetMessages;
using CrisesControl.Api.Application.Commands.Messaging.GetNotifications;
using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;
using CrisesControl.Api.Application.Commands.Messaging.GetPingInfo;
using CrisesControl.Api.Application.Commands.Messaging.GetReplies;
using CrisesControl.Api.Application.Commands.Messaging.MessageAcknowledged;
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

        /// <summary>
        /// Get the notification count for currently logged in user
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{CurrentUserId:int}/NotificationCount")]
        public async Task<IActionResult> GetNotificationsCount([FromRoute] GetNotificationsCountRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get single message response object by response id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMessageResponse/{MessageType}/{ResponseID:int}")]
        public async Task<IActionResult> GetMessageResponse([FromRoute] GetMessageResponseRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get the list of company message response for company by message type (ALL,Ping, Incident, Checklist)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMessageResponses/{MessageType}/{Status:int}")]
        public async Task<IActionResult> GetMessageResponses([FromRoute] GetMessageResponsesRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get the list of message for a user by message type (Ping or Incident)
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMessages/{TargetUserId:int}/{MessageType}/{IncidentActivationId:int}")]
        public async Task<IActionResult> GetMessages([FromRoute] GetMessagesRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get the  message details for a user by cloud message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMessageDetails/{CloudMsgId}/{MessageId}")]
        public async Task<IActionResult> GetMessageDetails([FromRoute] GetMessageDetailsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get the attachement of a message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMessageAttachment/{MessageListID}/{MessageID}")]
        public async Task<IActionResult> GetMessageAttachment([FromRoute]  GetMessageAttachmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Returns attachement by the attachement id
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttachment/{MessageAttachmentID}")]
        public async Task<IActionResult> GetAttachment([FromRoute] GetAttachmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get the acknowledge for a message
        /// </summary>
        /// <param name="requestRoute"></param>
        /// <param name="requestNullable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MessageAcknowledge/{CompanyId:int}/{CurrentUserId:int}/{MsgListId:int}/ResponseID")]
        public async Task<IActionResult> MessageAcknowledge([FromRoute] MessageAcknowledgeRequestRoute requestRoute, [FromQuery] MessageAcknowledgeRequestNullable requestNullable, CancellationToken cancellationToken)
        {
           
            MessageAcknowledgeRequest request = new MessageAcknowledgeRequest();
            request.AckMethod = requestNullable.AckMethod;
            request.UserLocationLat = requestNullable.UserLocationLat;
            request.UserLocationLong=requestNullable.UserLocationLong;
            request.CompanyId = requestRoute.CompanyId;
            request.CurrentUserId = requestRoute.CurrentUserId;
            request.ResponseID = requestRoute.ResponseID;
            request.MsgListId=requestRoute.MsgListId;

            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get message reply for a message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetReplies/{MessageId}")]
        public async Task<IActionResult> GetReplies([FromRoute] GetRepliesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get the list of message groups
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMessageGroupList/{MessageID}")]
        public async Task<IActionResult> GetMessageGroupList([FromRoute] GetMessageGroupListRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get conferece calls recordings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetConfRecordings")]
        public async Task<IActionResult> GetConfRecordings([FromBody] GetConfRecordingsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get conference users
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetConfUser")]
        public async Task<IActionResult> GetConfUser([FromBody] GetConfUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get notifications
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetNotifications")]
        public async Task<IActionResult> GetNotifications(GetNotificationsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get information about pings
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetPingInfo")]
        public async Task<IActionResult> GetPingInfo(GetPingInfoRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
