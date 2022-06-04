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

namespace CrisesControl.Api.Application.Query
{
    public class ReportsQuery : IReportsQuery
    {
        private readonly IReportsRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly string _timeZoneId = "GMT Standard Time";
        private readonly ILogger<ReportsQuery> _logger;
        private readonly IPaging _paging;

        public ReportsQuery(IReportsRepository reportRepository,
                            IMapper mapper,
                            ILogger<ReportsQuery> logger,
                            IPaging paging)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _logger = logger;
            _paging = paging;
        }

        public async Task<GetSOSItemsResponse> GetSOSItems(GetSOSItemsRequest request) {
            var sositems = await _reportRepository.GetSOSItems();

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
        public async Task<GetIndidentMessageAckResponse> GetIndidentMessageAck(GetIndidentMessageAckRequest request)
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
    }
}
