﻿using CrisesControl.Api.Application.Commands.Reports.GetFailedAttempts;
using CrisesControl.Api.Application.Commands.Reports.GetFailedTasks;
using CrisesControl.Api.Application.Commands.Reports.GetGroupPingReportChart;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentReport;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentStats;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentUserMessage;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentReportDetails;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary;
using CrisesControl.Api.Application.Commands.Reports.GetPerformanceReport;
using CrisesControl.Api.Application.Commands.Reports.GetPerformanceReportByGroup;
using CrisesControl.Api.Application.Commands.Reports.GetResponseCoordinates;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.GetTaskPerformance;
using CrisesControl.Api.Application.Commands.Reports.GetTrackingData;
using CrisesControl.Api.Application.Commands.Reports.GetTrackingUserCount;
using CrisesControl.Api.Application.Commands.Reports.GetUserIncidentReport;
using CrisesControl.Api.Application.Commands.Reports.GetUserReportPiechartData;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Api.Application.Query.Requests;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.SP_Response;

namespace CrisesControl.Api.Application.Query
{
    public interface IReportsQuery {
        public Task<GetSOSItemsResponse> GetSOSItems(GetSOSItemsRequest request);
        public Task<GetIncidentPingStatsResponse> GetIncidentPingStats(GetIncidentPingStatsRequest request);
        Task<GetIndidentMessageAckResponse> GetIndidentMessageAck(GetIndidentMessageAckRequest request);
        Task<ResponseSummaryResponse> ResponseSummary(ResponseSummaryRequest request);
        Task<GetIndidentMessageNoAckResponse> GetIndidentMessageNoAck(GetIndidentMessageNoAckRequest request);
        CurrentIncidentStatsResponse GetCurrentIncidentStats(int companyId);
        IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId);
        Task<GetMessageDeliveryReportResponse> GetMessageDeliveryReport(GetMessageDeliveryReportRequest request);
        DataTablePaging GetResponseReportByGroup(MessageReportRequest request, int companyId);
        List<IncidentMessageAuditResponse> GetIndidentMessagesAudit(int incidentActivationId, int companyId);
        List<IncidentUserLocationResponse> GetIncidentUserLocation(int incidentActivationId, int companyId);
        List<TrackUsers> GetTrackingUsers(string status, int userId, int companyId);
        Task<GetTrackingUserCountResponse> GetTrackingUserCount(GetTrackingUserCountRequest request);
        Task<GetMessageDeliverySummaryResponse> GetMessageDeliverySummary(GetMessageDeliverySummaryRequest request);
        Task<GetIndidentReportDetailsResponse> GetIndidentReportDetails(GetIndidentReportDetailsRequest request);
        Task<GetIncidentReportResponse> GetIncidentReport(GetIncidentReportRequest request);
        Task<GetUserReportPiechartDataResponse> GetUserReportPiechartData(GetUserReportPiechartDataRequest request);
        Task<GetUserIncidentReportResponse> GetUserIncidentReport(GetUserIncidentReportRequest request);
        Task<GetGroupPingReportChartResponse> GetGroupPingReportChart(GetGroupPingReportChartRequest request);
        Task<GetIncidentUserMessageResponse> GetIncidentUserMessage(GetIncidentUserMessageRequest request);
        Task<GetIncidentStatsResponse> GetIncidentStats(GetIncidentStatsRequest request);
        Task<GetPerformanceReportResponse> GetPerformanceReport(GetPerformanceReportRequest request);
        Task<GetPerformanceReportByGroupResponse> GetPerformanceReportByGroup(GetPerformanceReportByGroupRequest request);
        Task<GetResponseCoordinatesResponse> GetResponseCoordinates(GetResponseCoordinatesRequest request);
        Task<GetTrackingDataResponse> GetTrackingData(GetTrackingDataRequest request);
        Task<GetTaskPerformanceResponse> GetTaskPerformance(GetTaskPerformanceRequest request);
        Task<GetFailedTasksResponse> GetFailedTasks(GetFailedTasksRequest request);
        Task<GetFailedAttemptsResponse> GetFailedAttempts(GetFailedAttemptsRequest request);
    }
}
