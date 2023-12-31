﻿using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageNoAck;
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
using CrisesControl.Api.Application.Commands.Reports.GetIncidentReportDetails;
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
using CrisesControl.Api.Application.Commands.Reports.DownloadDeliveryReport;
using CrisesControl.Api.Application.Commands.Reports.GetUndeliveredMessage;
using CrisesControl.Api.Application.Commands.Reports.OffDutyReport;
using CrisesControl.Api.Application.Commands.Reports.ExportAcknowledgement;
using CrisesControl.Api.Application.Commands.Reports.NoAppUser;
using CrisesControl.Api.Application.Commands.Reports.IncidentResponseSummary;
using CrisesControl.Api.Application.Commands.Reports.GetMessageAnslysisResponse;
using CrisesControl.Api.Application.Commands.Reports.GetPingReportAnalysis;
using CrisesControl.Api.Application.Commands.Reports.AppInvitation;
using CrisesControl.Api.Application.Commands.Reports.GetCompanyCommunicationReport;
using CrisesControl.Api.Application.Commands.Reports.GetUserTracking;
using CrisesControl.Api.Application.Commands.Reports.CMD_TaskOverView;
using CrisesControl.Api.Application.Commands.Reports.IncidentResponseDump;
using CrisesControl.Api.Application.Commands.Reports.GetUserInvitationReport;
using CrisesControl.Api.Application.Commands.Reports.ExportUserInvitationDump;
using CrisesControl.Core.Compatibility;

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
        [Route("GetSOSItems/{UserId:int}")]
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
        [Route("GetIncidentMessageNoAck/{IncidentActivationId:int}/{RecordStart:int}/{RecordLength:int}")]
        public async Task<IActionResult> GetIncidentMessageNoAck([FromRoute] GetIncidentMessageNoAckRequest request, CancellationToken cancellationToken)
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
        [Route("GetIncidentMessageAck/{MessageId:int}/{MessageAckStatus:int}/{MessageSentStatus:int}")]
        public async Task<IActionResult> GetIncidentMessageAck([FromRoute] GetIncidentMessageAckRequest requestRoute, CancellationToken cancellationToken)
        {
            var result = await _reportQuery.GetIncidentMessageAck(requestRoute);

            return Ok(result);
        }
        
        /// <summary>
        /// Get the response summary
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ResponseSummary/{MessageId:int}")]
        public async Task<IActionResult> ResponseSummary([FromRoute] ResponseSummaryRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportQuery.ResponseSummary(request);

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
        public IActionResult GetIncidentMessagesAudit([FromRoute] int incidentActivationId, [FromRoute] int companyId)
        {
            var result = _reportQuery.GetIncidentMessagesAudit(incidentActivationId, companyId);
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
        [Route("GetIncidentReportDetails/{IncidentActivationId}")]
        public async Task<IActionResult> GetIncidentReportDetails([FromRoute] GetIncidentReportDetailsRequest request, CancellationToken cancellationToken)
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
        [HttpGet]
        [Route("DownloadDeliveryReport/{MessageID}")]
        public async Task<IActionResult> DownloadDeliveryReport([FromRoute] DownloadDeliveryReportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetUndeliveredMessage/{MessageID}/{CommsMethod}/{CountryCode}/{ReportType}/{CompanyKey}/{search}/{orderDir}/{Draw}")]
        public async Task<IActionResult> GetUndeliveredMessage([FromRoute] GetUndeliveredMessageRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("OffDutyReport/{CompanyId}")]
        public async Task<IActionResult> OffDutyReport([FromRoute] OffDutyReportRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("ExportAcknowledgement/{MessageId}/{CompanyKey}")]
        public async Task<IActionResult> ExportAcknowledgement([FromRoute] ExportAcknowledgementRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("NoAppUser/{CompanyId}/{MessageID}/{CompanyKey}/{search}/{orderDir}/{Draw}")]
        public async Task<IActionResult> NoAppUser([FromRoute] NoAppUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("IncidentResponseSummary/{ActiveIncidentID}")]
        public async Task<IActionResult> IncidentResponseSummary([FromRoute] IncidentResponseSummaryRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetMessageAnslysisResponse/{MessageId}/{DrillOpt}/{Search}/{orderDir}/{Draw}/{CompanyKey}")]
        public async Task<IActionResult> NoAppUser([FromRoute] GetMessageAnslysisResponseRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        [HttpGet]
        [Route("PingReportAnalysis/{MessageId:int}/{MessageType}")]
        public async Task<IActionResult> PingReportAnalysis([FromRoute] GetPingReportAnalysisRequest request, CancellationToken cancellationToken)
        {
            var result = await _reportQuery.PingReportAnalysis(request);
            return Ok(result);
        }
        [HttpGet]
        [Route("AppInvitation/{CompanyId:int}")]
        public async Task<IActionResult> AppInvitation([FromRoute] AppInvitationRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }
        /// <summary>
        /// Get company's communication reports
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCompanyCommunicationReport/{CompanyId:int}")]
        public async Task<IActionResult> GetCompanyCommunicationReport([FromRoute] GetCompanyCommunicationReportRequest request)
        {
            var result = await _reportQuery.GetCompanyCommunicationReport(request);
            return Ok(result);
        }
        /// <summary>
        /// Return user tracking report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserTracking/{Source}/{UserId:int}/{ActiveIncidentId:int}")]
        public async Task<IActionResult> GetUserTracking([FromRoute] GetUserTrackingRequest request)
        {
            var result = await _reportQuery.GetUserTracking(request);
            return Ok(result);
        }
        /// <summary>
        /// Return task overview
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CMD_TaskOverView/{IncidentActivationId:int}")]
        public async Task<IActionResult> CMD_TaskOverView([FromRoute] CMD_TaskOverViewRequest request)
        {
            var result = await _reportQuery.CMD_TaskOverView(request);
            return Ok(result);
        }
        /// <summary>
        /// Returns incident response
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Action")]
        public async Task<IActionResult> IncidentResponseDump([FromBody] IncidentResponseDumpRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        /// <summary>
        /// Get user invitation report
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetUserInvitationReport")]
        public async Task<IActionResult> GetUserInvitationReport([FromBody] GetUserInvitationReportRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
        /// <summary>
        /// Download user invitation detail
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ExportUserInvitationDump")]
        public async Task<IActionResult> ExportUserInvitationDump([FromBody] ExportUserInvitationDumpRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

    }
}
