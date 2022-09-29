using AutoMapper;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Query.Requests;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Core.Reports.SP_Response;
using CrisesControl.Api.Application.Commands.Reports.GetTrackingUserCount;
using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliverySummary;
using CrisesControl.Core.Exceptions.NotFound;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentReportDetails;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentReport;
using CrisesControl.Api.Application.Commands.Reports.GetUserReportPiechartData;
using CrisesControl.Api.Application.Commands.Reports.GetUserIncidentReport;
using CrisesControl.Api.Application.Commands.Reports.GetGroupPingReportChart;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentUserMessage;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentStats;
using CrisesControl.Api.Application.Commands.Reports.GetPerformanceReport;
using CrisesControl.SharedKernel.Utils;
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
using CrisesControl.Api.Application.Commands.Reports.IncidentResponseSummaries;
using CrisesControl.Api.Application.Commands.Reports.IncidentResponseDump;
using System.Data;
using CrisesControl.Api.Application.Commands.Reports.AppInvitation;
using CrisesControl.Api.Application.Commands.Reports.GetPingReportAnalysis;
using CrisesControl.Api.Application.Commands.Reports.GetMessageAnslysisResponse;
using CrisesControl.Api.Application.Commands.Reports.GetCompanyCommunicationReport;
using CrisesControl.Api.Application.Commands.Reports.GetUserTracking;
using CrisesControl.Api.Application.Commands.Reports.CMD_TaskOverView;
using CrisesControl.Api.Application.Commands.Reports.GetUserInvitationReport;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Api.Application.Commands.Reports.ExportUserInvitationDump;

namespace CrisesControl.Api.Application.Query
{
    public class ReportsQuery : IReportsQuery
    {
        private readonly IReportsRepository _reportRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        private readonly string _timeZoneId = "GMT Standard Time";
        private readonly ILogger<ReportsQuery> _logger;
        private readonly IPaging _paging;
        private readonly ICurrentUser _currentUser;
     
        public ReportsQuery(IReportsRepository reportRepository, IAdminRepository adminRepository, IMapper mapper,
            ILogger<ReportsQuery> logger, ICurrentUser currentUser, IPaging paging) {
            _mapper = mapper;
            _reportRepository = reportRepository;
            _currentUser = currentUser;
            _logger= logger;
            _paging= paging;
            _adminRepository = adminRepository;
           
        }

        public async Task<GetSOSItemsResponse> GetSOSItems(GetSOSItemsRequest request) {
            var sositems = await _reportRepository.GetSOSItems(request.UserId);

            var response = _mapper.Map<List<SOSItem>>(sositems);
            var result = new GetSOSItemsResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<GetIncidentPingStatsResponse> GetIncidentPingStats(GetIncidentPingStatsRequest request) {
            var stats = await _reportRepository.GetIncidentPingStats(request.CompanyId, request.NoOfMonth);

            var response = _mapper.Map<List<IncidentPingStatsCount>>(stats);
            var result = new GetIncidentPingStatsResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }
        public async Task<GetIndidentMessageAckResponse> GetIndidentMessageAck(GetIndidentMessageAckRequest request, DataTableAjaxPostModel pagedRequest)
        {
            var stats = await _reportRepository.GetIndidentMessageAck(request.MessageId, request.MessageAckStatus, request.MessageSentStatus, _paging.PageNumber, _paging.PageSize, request.SearchString,_paging.OrderBy,request.OrderDir,request.draw ?? default(int),request.Filters,request.CompanyKey, request.Source="WEB");

            var response = _mapper.Map<List<MessageAcknowledgements>>(stats);
            var result = new GetIndidentMessageAckResponse();
            result.Data = response;
            result.ErrorCode = System.Net.HttpStatusCode.OK;
            return result;

        }

        public async Task<ResponseSummaryResponse> ResponseSummary(ResponseSummaryRequest request)
        {
            var stats = await _reportRepository.ResponseSummary(request.MessageID);
            var response = _mapper.Map<List<ResponseSummary>>(stats);
            var result = new ResponseSummaryResponse();
            result.Data = response;
            result.ErrorCode = System.Net.HttpStatusCode.OK;
            return result;




        }
        public async Task<GetIndidentMessageNoAckResponse> GetIndidentMessageNoAck(GetIndidentMessageNoAckRequest request)
        {
            var stats = await _reportRepository.GetIndidentMessageNoAck(request.draw, request.IncidentActivationId, request.RecordStart, request.RecordLength, request.SearchString, request.UniqueKey);

            var response = _mapper.Map<List<DataTablePaging>>(stats);
            var result = new GetIndidentMessageNoAckResponse();
            result.data = response;
            result.StatusCode = System.Net.HttpStatusCode.OK;
            return result;
        }

        public CurrentIncidentStatsResponse GetCurrentIncidentStats(int companyId)
        {
            return _reportRepository.GetCurrentIncidentStats(_timeZoneId, companyId);
        }

        public IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId)
        {
            return _reportRepository.GetIncidentData(incidentActivationId, userId, companyId);
        }

        public async Task<GetMessageDeliveryReportResponse> GetMessageDeliveryReport(GetMessageDeliveryReportRequest request)
        {
            try
            {


                int totalRecord = 0;
                GetMessageDeliveryReportResponse rtn = new GetMessageDeliveryReportResponse();
                rtn.draw = request.draw;

                var Report = await _reportRepository.GetMessageDeliveryReport(request.StartDate, request.EndDate, _paging.PageNumber, _paging.PageSize, request.search ?? string.Empty,_paging.OrderBy, request.OrderDir , request.CompanyKey ?? string.Empty);
                var response = _mapper.Map<List<DeliveryOutput>>(Report);
                if (Report != null)
                {
                    totalRecord = Report.Count;
                    rtn.recordsFiltered = Report.Count;
                    rtn.data = response;
                }
                

                var TotalList = await _reportRepository.GetMessageDeliveryReport(request.StartDate, request.EndDate, 0, int.MaxValue, "", "M.MessageId", "desc", request.CompanyKey);
                var responseTotal = _mapper.Map<List<DeliveryOutput>>(TotalList);
                if (TotalList != null)
                {
                    totalRecord = TotalList.Count;
                }

                rtn.recordsTotal = totalRecord;
                return rtn;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error Occured while seeding into the database {0},{1},{2},{3},{4}",ex.Message,ex.StackTrace, ex.InnerException, ex.Source, ex.Data);
                return null;
            }
        }

        public DataTablePaging GetResponseReportByGroup(MessageReportRequest request, int companyId)
        {
            return _reportRepository.GetResponseReportByGroup((DataTableAjaxPostModel)request, request.StartDate, request.EndDate, request.MessageType, request.DrillOpt, request.GroupId, request.ObjectMappingId, request.CompanyKey, request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, companyId);
        }

        public List<IncidentMessageAuditResponse> GetIndidentMessagesAudit(int incidentActivationId, int companyId)
        {
            return _reportRepository.GetIndidentMessagesAudit(incidentActivationId, companyId);
        }

        public List<IncidentUserLocationResponse> GetIncidentUserLocation(int incidentActivationId, int companyId)
        {
            return _reportRepository.GetIncidentUserLocation(incidentActivationId, companyId);
        }

        public List<TrackUsers> GetTrackingUsers(string status, int userId, int companyId)
        {
            return _reportRepository.GetTrackingUsers(status, userId, companyId);
        }
        public async Task<GetTrackingUserCountResponse> GetTrackingUserCount(GetTrackingUserCountRequest request)
        {
           
                var trkUser = await _reportRepository.GetTrackingUserCount(_currentUser.CompanyId);

                var response = _mapper.Map<List<TrackUserCount>>(trkUser);
                var result = new GetTrackingUserCountResponse();
                if (trkUser != null)
                {
                    result.Data = response;
                    result.StatusCode = System.Net.HttpStatusCode.OK;
                    result.Message = "Data Loaded successfully";
                    return result;
                }

            throw new ReportingNotFoundException(_currentUser.CompanyId, _currentUser.UserId);

        }

        public async Task<GetMessageDeliverySummaryResponse> GetMessageDeliverySummary(GetMessageDeliverySummaryRequest request)
        {
            try
            {
                var summary = await _reportRepository.GetMessageDeliverySummary(request.MessageID);
                var response = _mapper.Map<List<DeliverySummary>>(summary);
                var result = new GetMessageDeliverySummaryResponse();
                result.Data = response;
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Message = "Summary has been Loaded";
                return result;
            }
            catch (Exception ex) {
                _logger.LogError("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                          ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return new GetMessageDeliverySummaryResponse();
            }

        }

        public async Task<GetIndidentReportDetailsResponse> GetIndidentReportDetails(GetIndidentReportDetailsRequest request)
        {
            try
            {
                var messagesRtns = await _reportRepository.GetIndidentReportDetails(request.IncidentActivationId, _currentUser.CompanyId,_currentUser.UserId);
                var response = _mapper.Map<List<IncidentMessagesRtn>>(messagesRtns);
                var result = new GetIndidentReportDetailsResponse();
                if (response != null)
                {
                    result.data = response;
                    result.Message = "Data Loaded";
                }
                else
                {
                    result.data = response;
                    result.Message = "No data found";
                }
               
                return result;
            }
            catch (Exception ex)
            {
                throw new ReportingNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
            }
        }
        public async Task<GetIncidentReportResponse> GetIncidentReport(GetIncidentReportRequest request)
        {
            try
            {
                var messagesRtns = await _reportRepository.GetIncidentReport(request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, request.StartDate, request.EndDate, _currentUser.UserId, _currentUser.CompanyId); ;
                var response = _mapper.Map<DataTablePaging>(messagesRtns);
                var result = new GetIncidentReportResponse();
                if (response != null)
                {
                    result.data = response;
                    result.Message = "Data Loaded";
                }
                else
                {
                    result.data = response;
                    result.Message = "No data found";
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ReportingNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
            }
        }

        public async Task<GetUserReportPiechartDataResponse> GetUserReportPiechartData(GetUserReportPiechartDataRequest request)
        {
            try
            {
                var messagesRtns = await _reportRepository.GetUserReportPiechartData(request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, request.StartDate, request.EndDate,_currentUser.UserId, _currentUser.CompanyId);
                var response = _mapper.Map<List<UserPieChart>>(messagesRtns);
                var result = new GetUserReportPiechartDataResponse();
                if (response != null)
                {
                    result.data = response;
                    result.Message = "Data Loaded";
                }
                else
                {
                    result.data = response;
                    result.Message = "No data found";
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ReportingNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
            }
        }

        public async Task<GetUserIncidentReportResponse> GetUserIncidentReport(GetUserIncidentReportRequest request)
        {
            try
            {
                var messagesRtns = await _reportRepository.GetUserIncidentReport(request.StartDate, request.EndDate,request.IsThisWeek, request.IsThisMonth, request.IsLastMonth,_currentUser.UserId);
              
                var result = new GetUserIncidentReportResponse();
                if (messagesRtns != null)
                {
                    result.data = messagesRtns;
                    result.Message = "Data Loaded";
                }
                else
                {
                    result.data = messagesRtns;
                    result.Message = "No data found";
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ReportingNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
            }
        }

        public async Task<GetGroupPingReportChartResponse> GetGroupPingReportChart(GetGroupPingReportChartRequest request)
        {
            try
            {
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                int FilterRelation = request.FilterRelation;

                _reportRepository.GetStartEndDate(request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, ref stDate, ref enDate, request.StartDate, request.EndDate);

               
                List<PingGroupChartCount> result = await _reportRepository.GetPingReportChart(stDate, enDate, request.FilterRelation, "PING", request.ObjectMappingID);

                var results = _mapper.Map<PingGroupChartCount>(result);
                var response = new GetGroupPingReportChartResponse();
                if (result != null)
                {
                    int PingMinLimit = Convert.ToInt32(await _reportRepository.GetCompanyParameter("MIN_PING_KPI", _currentUser.CompanyId));
                    int PingMaxLimit = Convert.ToInt32(await _reportRepository.GetCompanyParameter("MAX_PING_KPI", _currentUser.CompanyId));

                    var mainresult = result.FirstOrDefault();
                    mainresult.KPILimit = PingMinLimit;
                    mainresult.KPIMaxLimit = PingMaxLimit;
                    response.data = mainresult;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.data = results;
                    response.Message = "No data found";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new ReportingNotFoundException(_currentUser.CompanyId, _currentUser.UserId);
            }
        }

        public async Task<GetIncidentUserMessageResponse> GetIncidentUserMessage(GetIncidentUserMessageRequest request)
        {
            try
            {
                var messagesRtns = await _reportRepository.GetIncidentUserMessage(request.IncidentActivationId, _currentUser.UserId, _currentUser.CompanyId);
                var response = _mapper.Map<List<IncidentUserMessageResponse>>(messagesRtns);
                var result = new GetIncidentUserMessageResponse();
                if (response != null)
                {
                    result.data = response;
                    result.Message = "Data Loaded";
                }
                else
                {
                    result.data = response;
                    result.Message = "No data found";
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetIncidentStatsResponse> GetIncidentStats(GetIncidentStatsRequest request)
        {
            try
            {
                var messagesRtns = await _reportRepository.GetIncidentStats(request.IncidentActivationId, _currentUser.CompanyId);
                var response = _mapper.Map<IncidentStatsResponse>(messagesRtns);
                var result = new GetIncidentStatsResponse();
                if (response != null)
                {
                    result.Data = response;
                    result.Message = "Data Loaded";
                }
                else
                {
                    result.Data = response;
                    result.Message = "No data found";
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetPerformanceReportResponse> GetPerformanceReport(GetPerformanceReportRequest request)
        {
            try
            {
                var performances = await _reportRepository.GetPerformanceReport(request.IsThisWeek,request.IsThisMonth,request.IsLastMonth,request.ShowDeletedGroups,_currentUser.UserId, _currentUser.CompanyId,request.StartDate, request.EndDate,request.GroupType.ToGrString());
                var result = _mapper.Map<List<PerformanceReport>>(performances);
                var response = new GetPerformanceReportResponse();
                if (result != null)
                {
                    response.Data = performances;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = performances;
                    response.Message = "No data found";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetPerformanceReportByGroupResponse> GetPerformanceReportByGroup(GetPerformanceReportByGroupRequest request)
        {
            try
            {
    
                var dataTable = await _reportRepository.GetPerformanceReportByGroup(request.draw, request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, _currentUser.CompanyId,_paging.PageNumber,_paging.PageSize,request.SearchString,_paging.OrderBy,"asc", _currentUser.UserId,request.MessageType.ToDbMethodString(),request.GroupName, request.GroupType.ToGrString(),request.DrillOpt,  request.StartDate, request.EndDate, request.CompanyKey);
                var result = _mapper.Map<DataTablePaging>(dataTable);
                var response = new GetPerformanceReportByGroupResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No data found";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetResponseCoordinatesResponse> GetResponseCoordinates(GetResponseCoordinatesRequest request)
        {
            try
            {
                var coordinates = await _reportRepository.GetResponseCoordinates(_paging.PageNumber, _paging.PageSize, request.MessageId);
                var result = _mapper.Map<DataTablePaging>(coordinates);
                var response = new GetResponseCoordinatesResponse();
                if (result != null)
                {
                    response.Data = coordinates;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = coordinates;
                    response.Message = "No data found";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTrackingDataResponse> GetTrackingData(GetTrackingDataRequest request)
        {
            try
            {
                var track = await _reportRepository.GetTrackingData(request.TrackMeID, request.UserDeviceID, request.StartDate, request.EndDate);
                var result = _mapper.Map<List<TrackingExport>>(track);
                var response = new GetTrackingDataResponse();
                if (result != null)
                {
                    response.Data = track;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = track;
                    response.Message = "No data found";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTaskPerformanceResponse> GetTaskPerformance(GetTaskPerformanceRequest request)
        {
            try
            {
                var TaskReport = await _reportRepository.GetTaskPerformance(_currentUser.CompanyId,request.IsThisWeek, request.IsThisMonth, request.IsLastMonth,request.startDate,request.EndDate);
               
                var response = new GetTaskPerformanceResponse();
                if (TaskReport != null)
                {
                    var AcceptFailed = await _reportRepository.GetFailedTasks("Accept", _currentUser.CompanyId, request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, request.startDate, request.EndDate);
                    var CompleteFailed = await _reportRepository.GetFailedTasks("Completion", _currentUser.CompanyId, request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, request.startDate, request.EndDate);
                    response.Data = TaskReport;
                    response.failedTask = AcceptFailed;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = TaskReport;
                    response.Message = "No data found";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetFailedTasksResponse> GetFailedTasks(GetFailedTasksRequest request)
        {
            try
            {
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                _reportRepository.GetStartEndDate(request.IsThisWeek, request.IsThisMonth, request.IsLastMonth, ref stDate, ref enDate, request.startDate, request.EndDate);
                var response = new GetFailedTasksResponse();

                var RecordStart = _paging.PageNumber == 0 ? 0 : _paging.PageNumber;
                var RecordLength = _paging.PageSize == 0 ? int.MaxValue : _paging.PageSize;
                var SearchString = (request.Search != null) ? request.Search.ToString() :string.Empty;
                string OrderBy = _paging.OrderBy != null ? _paging.OrderBy.FirstOrDefault().ToString() : "TaskActivationDate";
                string OrderDir = request.dir != null ? request.dir.FirstOrDefault().ToString() : "desc";

                if (request.RangeMax == -1)
                    request.RangeMax = int.MaxValue;

                int totalRecord = 0;
                DataTablePaging rtn = new DataTablePaging();
                rtn.Draw = request.draw;
                var TaskReport = await _reportRepository.GetFailedTaskList(request.ReportType, request.RangeMin, request.RangeMax, stDate, enDate, RecordStart, RecordLength,
                       SearchString, OrderBy, OrderDir, request.CompanyKey, _currentUser.CompanyId);

                if (TaskReport != null)
                {
                    totalRecord = TaskReport.Count;
                    rtn.RecordsFiltered = TaskReport.Count;
                    rtn.Data = TaskReport;
                }
                var TotalList =await _reportRepository.GetFailedTaskList(request.ReportType, request.RangeMin, request.RangeMax, stDate, enDate, 0, int.MaxValue,
                       "", "TaskActivationDate", "asc", request.CompanyKey, _currentUser.CompanyId);
                if (TotalList != null)
                {
                    totalRecord = TotalList.Count;
                }

                rtn.RecordsTotal = totalRecord;
                if (rtn.Data != null)
                {
                    response.Data = rtn;
                    response.Message = "Data has been Loaded";
                }
                else
                {
                    response.Data = rtn;
                    response.Message = "No record found.";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetFailedAttemptsResponse> GetFailedAttempts(GetFailedAttemptsRequest request)
        {
            try
            {
                var track = await _reportRepository.GetFailedAttempts(request.MessageListID, request.CommsMethod);
                var result = _mapper.Map<List<FailedAttempts>>(track);
                var response = new GetFailedAttemptsResponse();
                if (result != null)
                {
                    response.Data = track;
                    response.Message = "Data Loaded";
                }
                else
                {
                    response.Data = track;
                    response.Message = "No data found";
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DownloadDeliveryReportResponse> DownloadDeliveryReport(DownloadDeliveryReportRequest request)
        {
            try
            {
                var track = await _reportRepository.DownloadDeliveryReport(_currentUser.CompanyId,request.MessageID, _currentUser.UserId);
                var result = _mapper.Map<string>(track);
                var response = new DownloadDeliveryReportResponse();
                if (!string.IsNullOrEmpty(result))
                {
                    response.FileName = result;
                  
                }
                else
                {
                    response.FileName = result;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetUndeliveredMessageResponse> GetUndeliveredMessage(GetUndeliveredMessageRequest request)
        {
            try
            {
                var RecordStart = _paging.PageNumber == 0 ? 0 : _paging.PageNumber;
                var RecordLength = _paging.PageSize == 0 ? int.MaxValue : _paging.PageSize;
                var SearchString = (request.search != null) ? request.search : string.Empty;
                string OrderBy = _paging.OrderBy != null ? _paging.OrderBy.FirstOrDefault().ToString() : "MT.MessageListID";
                string OrderDir = request.orderDir != null ? request.orderDir.FirstOrDefault().ToString() : "desc";

                int totalRecord = 0;
                DataTablePaging rtn = new DataTablePaging();
                rtn.Draw = request.Draw;

                var Report = await _reportRepository.GetUndeliveredMessage(request.MessageID, request.CommsMethod, request.CountryCode, request.ReportType, RecordStart, RecordLength,
                        SearchString, OrderBy, OrderDir, request.CompanyKey);

                if (Report != null)
                {
                    totalRecord = Report.Count;
                    rtn.RecordsFiltered = Report.Count;
                    rtn.Data = Report;
                }

                var TotalList = await _reportRepository.GetUndeliveredMessage(request.MessageID, request.CommsMethod, request.CountryCode, request.ReportType, 0, int.MaxValue, "", "MT.MessageListID", "asc", request.CompanyKey);

                if (TotalList != null)
                {
                    totalRecord = TotalList.Count;
                }

                rtn.RecordsTotal = totalRecord;


                var result = _mapper.Map<List<DeliveryOutput>>(Report);
                var response = new GetUndeliveredMessageResponse();
                if (rtn.Data!=null)
                {
                    response.Data = rtn;

                }
                else
                {
                    response.Data = rtn;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OffDutyReportResponse> OffDutyReport(OffDutyReportRequest request)
        {
            try
            {
                var offDuty = await _reportRepository.OffDutyReport(request.CompanyId,  _currentUser.UserId);
                var result = _mapper.Map<List<UserItems>>(offDuty);
                var response = new OffDutyReportResponse();
                if (result!=null)
                {
                    response.Data = result;

                }
                else
                {
                    response.Data = result;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ExportAcknowledgementResponse> ExportAcknowledgement(ExportAcknowledgementRequest request)
        {
            try
            {
                var exportReport = await _reportRepository.ExportAcknowledgement(request.MessageId, _currentUser.CompanyId, _currentUser.UserId, request.CompanyKey);
                var result = _mapper.Map<string>(exportReport); 
                var response = new ExportAcknowledgementResponse();
                if (string.IsNullOrEmpty(result))
                {
                    response.FileName = result;

                }
                else
                {
                    response.FileName = result;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<NoAppUserResponse> NoAppUser(NoAppUserRequest request)
        {
            try
            {
                var RecordStart = _paging.PageNumber == 0 ? 0 : _paging.PageNumber;
                var RecordLength = _paging.PageSize == 0 ? int.MaxValue : _paging.PageSize;
                var SearchString = (request.search != null) ? request.search : string.Empty;
                string OrderBy = _paging.OrderBy != null ? _paging.OrderBy.FirstOrDefault().ToString() : "U.UserId";
                string OrderDir = request.orderDir != null ? request.orderDir.FirstOrDefault().ToString() : "desc";

                int totalRecord = 0;
                DataTablePaging rtn = new DataTablePaging();
                rtn.Draw = request.Draw;

                var Report = await _reportRepository.NoAppUser(request.CompanyId, request.MessageID, RecordStart, RecordLength, SearchString, OrderBy, OrderDir, request.CompanyKey); 

                if (Report != null)
                {
                    totalRecord = Report.Count();
                    rtn.RecordsFiltered = Report.Count();
                    rtn.Data = Report;
                }

                var TotalList = await _reportRepository.NoAppUser(request.CompanyId, request.MessageID, 0, int.MaxValue, "", "U.UserId", "asc", request.CompanyKey);

                if (TotalList != null)
                {
                    totalRecord = TotalList.Count();
                }

                rtn.RecordsTotal = totalRecord;


                var result = _mapper.Map<DataTablePaging>(rtn);
                var response = new NoAppUserResponse();
                if (rtn.Data != null)
                {
                    response.Data = rtn;

                }
                else
                {
                    response.Data = rtn;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IncidentResponseSummaryResponse> IncidentResponseSummary(IncidentResponseSummaryRequest request)
        {
            try
            {
                var exportReport = await _reportRepository.IncidentResponseSummary(request.ActiveIncidentID);
                var result = _mapper.Map<List<IncidentResponseSummary>>(exportReport);
                var response = new IncidentResponseSummaryResponse();
                if (result!=null)
                {
                    response.Data = result;

                }
                else
                {
                    response.Data = result;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IncidentResponseDumpResponse> IncidentResponseDump(IncidentResponseDumpRequest request)
        {
            try
            {
                string rFilePath = string.Empty;
                string rFileName = string.Empty;
                DataTable dataTable = _reportRepository.GetReportData(request.IncidentActivationId, out rFilePath, out rFileName);
                
                var result = _mapper.Map<DataTable>(dataTable);
                var response = new IncidentResponseDumpResponse();
                var data = await _reportRepository.ToCSVHighPerformance(result, true, ",");
                using (StreamWriter SW = new StreamWriter(rFilePath, false))
                {
                    SW.Write(data);
                }
                if (data != null)
                {
                    response.filename = rFileName;

                }
                else
                {
                    response.filename = rFileName;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AppInvitationResponse> AppInvitation(AppInvitationRequest request)
        {
            try
            {
               await Task.Factory.StartNew(async () => await _reportRepository.AppInvitation(request.CompanyId));
              
                var response = new AppInvitationResponse();
                 response.isAppInvited = true;
              
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetPingReportAnalysisResponse> PingReportAnalysis(GetPingReportAnalysisRequest request)
        {
            try
            {
                DateTime stDate = DateTime.Now;
                DateTime enDate = DateTime.Now;
                List<PingReport> result =await _reportRepository.GetPingReportAnalysis(request.MessageId, request.MessageType.ToDbString(), _currentUser.CompanyId);

                var response = new GetPingReportAnalysisResponse();
               
                if (result != null)
                {
                    int PingMinLimit = Convert.ToInt32(await _reportRepository.GetCompanyParameter("MIN_PING_KPI", _currentUser.CompanyId));

                    int PingMaxLimit = Convert.ToInt32(await _reportRepository.GetCompanyParameter("MAX_PING_KPI", _currentUser.CompanyId));

                    var mainresult = result.FirstOrDefault();
                    mainresult.KPILimit = PingMinLimit;
                    mainresult.KPIMaxLimit = PingMaxLimit;
                    response.Data = mainresult;
                    response.Message = "No record found.";

                }
                else
                {
                    response.Data =new PingReport();
                    response.Message = "No record found.";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async  Task<GetMessageAnslysisResponseResponse> GetMessageAnslysisResponse(GetMessageAnslysisResponseRequest request)
        {
            try
            {
                var exportReport = await _reportRepository.GetMessageAnslysisResponse(_currentUser.CompanyId,request.MessageId, request.MessageType.ToDbString(),request.DrillOpt, _paging.PageNumber, _paging.PageSize, request.Search,_paging.OrderBy,request.orderDir,request.CompanyKey, request.Draw);
                var result = _mapper.Map<DataTablePaging>(exportReport);
                var response = new GetMessageAnslysisResponseResponse();
                if (result != null)
                {
                    response.Data = result;

                }
                else
                {
                    response.Data = result;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetCompanyCommunicationReportResponse> GetCompanyCommunicationReport(GetCompanyCommunicationReportRequest request)
        {
            try
            {
                var companyCommunication = await _reportRepository.GetCompanyCommunicationReport(request.CompanyId);
                GetCompanyCommunicationReportResponse response = _mapper.Map<GetCompanyCommunicationReportResponse>(companyCommunication);
                return response;
            } catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<GetUserTrackingResponse>> GetUserTracking(GetUserTrackingRequest request)
        {
            try
            {
                var result = await _reportRepository.GetUserTracking(request.Source, request.UserId, request.ActiveIncidentId);
                List<GetUserTrackingResponse> response = _mapper.Map<List<GetUserTrackingResponse>>(result);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CMD_TaskOverViewResponse> CMD_TaskOverView(CMD_TaskOverViewRequest request)
        {
            try
            {
                var result = await _reportRepository.CMD_TaskOverView(request.IncidentActivationId);
                CMD_TaskOverViewResponse response = _mapper.Map<CMD_TaskOverViewResponse>(result);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetUserInvitationReportResponse> GetUserInvitationReport(GetUserInvitationReportRequest request)
        {
            try
            {
                UserInvitationModel mappedRequst = _mapper.Map<UserInvitationModel>(request);
                var result = await _reportRepository.GetUserInvitationReport(mappedRequst);
                GetUserInvitationReportResponse response = _mapper.Map<GetUserInvitationReportResponse>(result);
                return response;
            } catch (Exception ex)
            {
                throw ex;
            }
        }

        public ExportUserInvitationDumpResponse ExportUserInvitationDump(ExportUserInvitationDumpRequest request)
        {
            try
            {
                var response = new ExportUserInvitationDumpResponse();
                string rFilePath, rFileName;
                UserInvitationModel mappedRequst = _mapper.Map<UserInvitationModel>(request);
                DataTable dataTable = _reportRepository.GetUserInvitationReportData(mappedRequst, out rFilePath, out rFileName);


                var data = _adminRepository.ToCSVHighPerformance(dataTable, true, ",");
                using (StreamWriter SW = new StreamWriter(rFilePath, false))
                {
                    SW.Write(data);
                }
                response.FileName = rFileName;
                return response;
            }
            catch(Exception ex) { throw ex; }
        }

    }
}
