using CrisesControl.Api.Application.Commands.Communication.DeleteRecording;
using CrisesControl.Api.Application.Commands.Communication.DownloadRecording;
using CrisesControl.Api.Application.Commands.Communication.GetUserActiveConferences;
using CrisesControl.Api.Application.Commands.Communication.HandelCallResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelCMSMSResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelConfRecording;
using CrisesControl.Api.Application.Commands.Communication.HandelConfResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelPushResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelSMSResponse;
using CrisesControl.Api.Application.Commands.Communication.HandelTwoFactor;
using CrisesControl.Api.Application.Commands.Communication.HandelUnifonicCallResponse;
using CrisesControl.Api.Application.Commands.Communication.StartConference;
using CrisesControl.Api.Application.Commands.Communication.TwilioCall;
using CrisesControl.Api.Application.Commands.Communication.TwilioCallAck;
using CrisesControl.Api.Application.Commands.Communication.TwilioCallLog;
using CrisesControl.Api.Application.Commands.Communication.TwilioConfLog;
using CrisesControl.Api.Application.Commands.Communication.TwilioEndConferenceCall;
using CrisesControl.Api.Application.Commands.Communication.TwilioRecordingLog;
using CrisesControl.Api.Application.Commands.Communication.TwilioText;
using CrisesControl.Api.Application.Commands.Communication.TwilioTextLog;
using CrisesControl.Api.Application.Commands.Communication.TwilioVerify;
using CrisesControl.Api.Application.Commands.Communication.TwilioVerifyCheck;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommunicationController : Controller {
        private readonly IMediator _mediator;

        public CommunicationController(IMediator mediator) {
            _mediator = mediator;
        }
        /// <summary>
        /// Get User Active Conferences
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserActiveConferences/{CompanyId:int}")]
        public async Task<IActionResult> GetUserActiveConferences([FromRoute] GetUserActiveConferencesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Twilio End Conference Call
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioEndConferenceCall([FromBody] TwilioEndConferenceCallRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Verify Check
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioVerifyCheck([FromBody] TwilioVerifyCheckRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Verify
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioVerify([FromBody] TwilioVerifyRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Handel Two Factor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> HandelTwoFactor([FromBody] HandelTwoFactorRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Recording Log
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioRecordingLog([FromBody] TwilioRecordingLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Conference Log
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioConfLog([FromBody] TwilioConfLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Call Log
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioCallLog([FromBody] TwilioCallLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Text Log
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioTextLog([FromBody] TwilioTextLogRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Delete Recording
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteRecording([FromBody] DeleteRecordingRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Call
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioCall([FromBody] TwilioCallRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Text
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TwilioText([FromBody] TwilioTextRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Download Recording
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DownloadRecording/FileName={FileName}")]
        public async Task<IActionResult> DownloadRecording([FromRoute] DownloadRecordingRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Start Conference
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> StartConference([FromRoute] StartConferenceRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Handel Conference Recording
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> HandelConfRecording([FromBody] HandelConfRecordingRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Handel Conference Response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> HandelConfResponse([FromBody] HandelConfResponseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Handel Push Response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]/{SendBackId}")]
        public async Task<IActionResult> HandelPushResponse([FromBody] HandelPushResponseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Handel CM SMS Response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("HandelCMSMSResponse/MessageSid={MessageSid}&Status={Status}&To={To}&Body={Body}")]
        public async Task<IActionResult> HandelCMSMSResponse([FromRoute] HandelCMSMSResponseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Handel SMS Response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("HandelSMSResponse/MessageSid={MessageSid}&Status={Status}")]
        public async Task<IActionResult> HandelSMSResponse([FromRoute] HandelSMSResponseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Twilio Call Ack
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("TwilioCallAck")]
        public async Task<IActionResult> TwilioCallAck([FromRoute] TwilioCallAckRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Handel Unifonicial Call Response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> HandelUnifonicCallResponse([FromBody] HandelUnifonicCallResponseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Handel Call Response
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> HandelCallResponse([FromBody] HandelCallResponseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
    }
}
