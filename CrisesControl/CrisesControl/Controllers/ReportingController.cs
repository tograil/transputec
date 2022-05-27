using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Api.Application.Commands.Reports.GetPingReportChart;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Api.Application.Helpers;
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
        private readonly ICurrentUser _currentUser;

        public ReportingController(IMediator mediator, IReportsQuery reportQuery, ICurrentUser currentUser) {
            _mediator = mediator;
            _reportQuery = reportQuery;
            _currentUser = currentUser;
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
        [Route("GetIndidentMessageAck/{MessageId:int}/{MessageAckStatus:int}/{MessageSentStatus:int}")]
        public async Task<IActionResult> GetIndidentMessageAck([FromRoute] IncidentMsgAckRequestRoute requestRoute,
            [FromQuery] IncidentMsgAckRequestQuery requestQuery, CancellationToken cancellationToken)
        {
            GetIndidentMessageAckRequest request = new GetIndidentMessageAckRequest();
            request.MessageId = requestRoute.MessageId;
            request.MessageAckStatus = requestRoute.MessageAckStatus;
            request.MessageSentStatus = requestRoute.MessageSentStatus;
            request.SearchString = requestQuery.SearchString;
            request.OrderDir = requestQuery.OrderDir;
            request.Source = requestQuery.Source;
            request.draw = requestQuery.draw;
            request.CompanyKey = requestQuery.CompanyKey;
            request.Filters = requestQuery.Filters;

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
        public async Task<IActionResult> ResponseSummary([FromRoute] ResponseSummaryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }
        [HttpGet]
        [Route("GetUserPingReportBarChart/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetUserPingReportBarChart([FromRoute] GetPingReportChartRequest request, CancellationToken cancellationToken)
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
        [Route("GetMessageDeliveryReport/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetMessageDeliveryReport([FromRoute] GetMessageDeliveryReportRouteRequest routeRequest, [FromQuery] GetMessageDeliveryReportQueryRequest queryRequest, CancellationToken cancellationToken)
        {
            GetMessageDeliveryReportRequest request = new GetMessageDeliveryReportRequest();
            request.CompanyKey = queryRequest.CompanyKey;
            request.draw = queryRequest.draw;
            request.search = queryRequest.search;
            request.OrderDir = queryRequest.OrderDir;
            request.StartDate = routeRequest.StartDate;
            request.EndDate = routeRequest.EndDate;
            var result = await _mediator.Send(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCurrentIncidentStats()
        {
            var result = _reportQuery.GetCurrentIncidentStats();
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]/{incidentActivationId:int}")]
        public IActionResult GetIncidentData(int incidentActivationId)
        {
            var result = _reportQuery.GetIncidentData(incidentActivationId, _currentUser.UserId, _currentUser.CompanyId);
            return Ok(result);
        }

    }
}
