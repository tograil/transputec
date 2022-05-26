using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Api.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrisesControl.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportingController : Controller {
        private readonly IMediator _mediator;
        private readonly IReportsQuery _reportQuery;

        public ReportingController(IMediator mediator, IReportsQuery reportQuery) {
            _mediator = mediator;
            _reportQuery = reportQuery;
        }

        /// <summary>
        /// Get SOS Items for a user.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSOSItems")]
        public async Task<IActionResult> GetSOSItems([FromRoute] GetSOSItemsRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get the stats data for the dashboard charts
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetIncidentPingStats/{CompanyId:int}/{NoOfMonth:int}")]
        public async Task<IActionResult> GetIncidentPingStats([FromRoute] GetIncidentPingStatsRequest request, CancellationToken cancellationToken) {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("GetIndidentMessageNoAck/{IncidentActivationId:int}/{RecordStart:int}/{RecordLength:int}")]
        public async Task<IActionResult> GetIndidentMessageNoAck([FromRoute] GetIndidentMessageNoAckRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Get the indident message Acknowledgement
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]     
        [Route("GetIndidentMessageAck/{CompanyID:int}/{MessageId:int}/{MessageAckStatus:int}/{MessageSentStatus:int}/{RecordStart:int}/{RecordLength:int}/{SearchString}")]
        public async Task<IActionResult> GetIndidentMessageAck([FromRoute] GetIndidentMessageAckRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get the response summary
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ResponseSummary/{MessageID:int}")]
        public async Task<IActionResult> ResponseSummary([FromQuery] ResponseSummaryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        /// <summary>
        /// Get Message Delivery Summary
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMessageDeliverySummary/{MessageID:int}")]
        public async Task<IActionResult> GetMessageDeliverySummary([FromRoute] GetMessageDeliverySummaryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

    }
}
