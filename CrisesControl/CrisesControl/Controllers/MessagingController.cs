using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponse;
using CrisesControl.Api.Application.Commands.Messaging.GetMessageResponses;
using CrisesControl.Api.Application.Commands.Messaging.GetMessages;
using CrisesControl.Api.Application.Commands.Messaging.GetNotificationsCount;
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
    }
}
