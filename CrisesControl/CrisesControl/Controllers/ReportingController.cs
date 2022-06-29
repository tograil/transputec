using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport;
using CrisesControl.Api.Application.Commands.Reports.GetPingReportChart;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.GetTrackingUserCount;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Query;
using CrisesControl.Api.Application.Query.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentReportDetails;
using CrisesControl.Api.Application.Commands.Reports.GetUserReportPiechartData;
using CrisesControl.Api.Application.Commands.Reports.GetUserIncidentReport;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentReport;
using CrisesControl.Api.Application.Commands.Reports.GetGroupPingReportChart;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentUserMessage;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentStats;
using CrisesControl.Api.Application.Commands.Reports.GetPerformanceReport;
using CrisesControl.Api.Application.Commands.Reports.GetPerformanceReportByGroup;
using CrisesControl.Api.Application.Commands.Reports.GetResponseCoordinates;
using CrisesControl.Api.Application.Commands.Reports.GetTrackingData;
using CrisesControl.Api.Application.Commands.Reports.GetTaskPerformance;
using CrisesControl.Api.Application.Commands.Reports.GetFailedTasks;
using CrisesControl.Api.Application.Commands.Reports.GetFailedAttempts;

namespace CrisesControl.Api.Controllers
{
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
            var result = _reportQuery.GetCurrentIncidentStats(_currentUser.CompanyId);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]/{incidentActivationId:int}")]
        public IActionResult GetIncidentData([FromRoute] int incidentActivationId)
        {
            var result = _reportQuery.GetIncidentData(incidentActivationId, _currentUser.UserId, _currentUser.CompanyId);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetResponseReportByGroup([FromQuery] MessageReportRequest request)
        {
            var result = _reportQuery.GetResponseReportByGroup(request, _currentUser.CompanyId);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]/{incidentActivationId:int}/{companyId:int}")]
        public IActionResult GetIndidentMessagesAudit([FromRoute] int incidentActivationId, [FromRoute] int companyId)
        {
            var result = _reportQuery.GetIndidentMessagesAudit(incidentActivationId, companyId);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]/{incidentActivationId:int}/{companyId:int}")]
        public IActionResult GetIncidentUserLocation([FromRoute] int incidentActivationId, [FromRoute] int companyId)
        {
            var result = _reportQuery.GetIncidentUserLocation(incidentActivationId, companyId);
            return Ok(result);
        }

        [HttpGet]
        [Route("[action]/{status}")]
        public IActionResult GetTrackingUsers([FromRoute] string status)
        {
            var result = _reportQuery.GetTrackingUsers(status, _currentUser.UserId, _currentUser.CompanyId);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetTrackingUserCount")]
        public async Task<IActionResult> GetTrackingUserCount([FromRoute] GetTrackingUserCountRequest request, CancellationToken cancellationToken)
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
        [HttpGet]
        [Route("GetIndidentReportDetails/{IncidentActivationId}")]
        public async Task<IActionResult> GetIndidentReportDetails([FromRoute] GetIndidentReportDetailsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetUserReportPiechartData/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetUserReportPiechartData([FromRoute] GetUserReportPiechartDataRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetUserIncidentReport/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetUserIncidentReport([FromRoute] GetUserIncidentReportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetIncidentReport/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetIncidentReport([FromRoute] GetIncidentReportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetGroupPingReportChart/{StartDate}/{EndDate}/{GroupID}")]
        public async Task<IActionResult> GetGroupPingReportChart([FromRoute] GetGroupPingReportChartRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetIncidentUserMessage/{IncidentActivationId}")]
        public async Task<IActionResult> GetIncidentUserMessage([FromRoute] GetIncidentUserMessageRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetIncidentStats/{IncidentActivationId}")]
        public async Task<IActionResult> GetIncidentStats([FromRoute] GetIncidentStatsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetPerformanceReport/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetPerformanceReport([FromRoute] GetPerformanceReportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetPerformanceReportByGroup/{IsThisWeek}/{IsThisMonth}/{IsLastMonth}/{MessageType}/{GroupName}/{GroupType}/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetPerformanceReportByGroup([FromRoute] GetPerformanceReportByGroupRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetResponseCoordinates/{MessageId}")]
        public async Task<IActionResult> GetResponseCoordinates([FromRoute] GetResponseCoordinatesRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetTrackingData/{TrackMeID}/{UserDeviceID}/{StartDate}/{EndDate}")]
        public async Task<IActionResult> GetTrackingData([FromRoute] GetTrackingDataRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetTaskPerformance/{IsThisWeek}/{IsThisMonth}/{IsLastMonth}/{startDate}/{EndDate}")]
        public async Task<IActionResult> GetTaskPerformance([FromRoute] GetTaskPerformanceRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetFailedTasks/{IsThisWeek}/{IsThisMonth}/{IsLastMonth}/{startDate}/{EndDate}/{RangeMax}/{RangeMin}/{ReportType}/{Search}/{dir}")]
        public async Task<IActionResult> GetFailedTasks([FromRoute] GetFailedTasksRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetFailedAttempts/{MessageListID}/{CommsMethod}")]
        public async Task<IActionResult> GetFailedAttempts([FromRoute] GetFailedAttemptsRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

    }
}
