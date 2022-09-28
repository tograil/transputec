using CrisesControl.Api.Application.Commands.Messaging.ConvertToMp3;
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
using CrisesControl.Api.Application.Commands.Messaging.GetPublicAlert;
using CrisesControl.Api.Application.Commands.Messaging.GetPublicAlertTemplate;
using CrisesControl.Api.Application.Commands.Messaging.GetReplies;
using CrisesControl.Api.Application.Commands.Messaging.MessageAcknowledged;
using CrisesControl.Api.Application.Commands.Messaging.PingMessage;
using CrisesControl.Api.Application.Commands.Messaging.ProcessPAFile;
using CrisesControl.Api.Application.Commands.Messaging.ReplyToMessage;
using CrisesControl.Api.Application.Commands.Messaging.ResendFailed;
using CrisesControl.Api.Application.Commands.Messaging.SaveMessageResponse;
using CrisesControl.Api.Application.Commands.Messaging.SendPublicAlert;
using CrisesControl.Api.Application.Commands.Messaging.StartConference;
using CrisesControl.Api.Application.Commands.Messaging.UploadAttachment;
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
        [HttpPut]
        [Route("MessageAcknowledge/{MsgListId:int}")]
        public async Task<IActionResult> MessageAcknowledge([FromRoute] int MsgListId, [FromBody] MessageAcknowledgeRequestRoute requestRoute, CancellationToken cancellationToken)
        {

            MessageAcknowledgeRequest request = new MessageAcknowledgeRequest();
            request.AckMethod = requestRoute.AckMethod;
            request.UserLocationLat = requestRoute.UserLocationLat;
            request.UserLocationLong = requestRoute.UserLocationLong;
            request.ResponseID = requestRoute.ResponseID;
            request.MsgListId = MsgListId;

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
        [HttpGet]
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
        [HttpGet]
        [Route("[action]/{ObjectType}/{ObjectId}")]
        public async Task<IActionResult> GetConfUser([FromRoute] GetConfUserRequest request, CancellationToken cancellationToken)
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
        [HttpGet]
        [Route("GetNotifications")]
        public async Task<IActionResult> GetNotifications([FromRoute] GetNotificationsRequest request, CancellationToken cancellationToken)
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
        [HttpGet]
        [Route("GetPingInfo/{MessageId}")]
        public async Task<IActionResult> GetPingInfo([FromRoute] GetPingInfoRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Ping Message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("PingMessage")]
        public async Task<IActionResult> PingMessage([FromBody] PingMessageRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Start conference
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> StartConference([FromBody] StartConferenceRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Save message response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveMessageResponse")]
        public async Task<IActionResult> SaveMessageResponse([FromBody] SaveMessageResponseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Resend failed message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ResendFailed")]
        public async Task<IActionResult> ResendFailed([FromBody] ResendFailedRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Upload attachment
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadAttachment")]
        public async Task<IActionResult> UploadAttachment([FromBody]  UploadAttachmentRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Reply to message
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ReplyToMessage")]
        public async Task<IActionResult> ReplyToMessage([FromBody] ReplyToMessageRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Send public alert
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendPublicAlert")]
        public async Task<IActionResult> SendPublicAlert([FromBody] SendPublicAlertRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get the template for public alert
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPublicAlertTemplate/{MessageId}")]
        public async Task<IActionResult> GetPublicAlertTemplate([FromRoute] GetPublicAlertTemplateRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get public alert
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPublicAlert")]
        public async Task<IActionResult> GetPublicAlert([FromRoute] GetPublicAlertRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Process PA file
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ProcessPAFile")]
        public async Task<IActionResult> ProcessPAFile([FromBody] ProcessPAFileRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Covert file to MP3
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ConvertToMp3")]
        public async Task<IActionResult> ConvertToMp3([FromBody] ConvertToMp3Request request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
