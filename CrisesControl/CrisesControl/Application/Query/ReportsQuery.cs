using AutoMapper;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;

using CrisesControl.Api.Application.Commands.Reports.GetMessageDeliveryReport;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Compatibility;

namespace CrisesControl.Api.Application.Query
{
    public class ReportsQuery : IReportsQuery 
    {
        private readonly IReportsRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportsQuery> _logger;
        private readonly ICurrentUser _currentUser;

        public ReportsQuery(IReportsRepository reportRepository, IMapper mapper,
            ILogger<ReportsQuery> logger, ICurrentUser currentUser) {
            _mapper = mapper;
            _reportRepository = reportRepository;
            _currentUser = currentUser;
            _logger= logger;
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
            var stats = await _reportRepository.GetIncidentPingStats(request.CompanyId,request.NoOfMonth);

            var response = _mapper.Map<List<IncidentPingStatsCount>>(stats);
            var result = new GetIncidentPingStatsResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }
        public async Task<GetIndidentMessageAckResponse> GetIndidentMessageAck(GetIndidentMessageAckRequest request)
        {
            var stats = await _reportRepository.GetIndidentMessageAck(request.MessageId, request.MessageAckStatus, request.MessageSentStatus, request.Start,request.Length,request.SearchString,request.order,request.draw ?? default(int),request.Filters,request.CompanyKey, request.Source="WEB");

            var response = _mapper.Map<List<MessageAcknowledgements>>(stats);
            var result = new GetIndidentMessageAckResponse();
            result.Data = response;
            result.ErrorCode =System.Net.HttpStatusCode.OK;
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
            var stats = await _reportRepository.GetIndidentMessageNoAck(request.draw,request.IncidentActivationId, request.RecordStart, request.RecordLength,request.SearchString, request.UniqueKey);

            var response = _mapper.Map<List<DataTablePaging>>(stats);
            var result = new GetIndidentMessageNoAckResponse();
            result.data = response;
            result.StatusCode =System.Net.HttpStatusCode.OK ;
            return result;
        }       
        

        public async Task<GetMessageDeliveryReportResponse> GetMessageDeliveryReport(GetMessageDeliveryReportRequest request)
        {
            try
            {


                int totalRecord = 0;
                GetMessageDeliveryReportResponse rtn = new GetMessageDeliveryReportResponse();
                rtn.draw = request.draw;

                var Report = await _reportRepository.GetMessageDeliveryReport(request.StartDate, request.EndDate, request.start ?? 0, request.length ?? 0, request.search ?? string.Empty, request.order , request.CompanyKey ?? string.Empty);
                var response = _mapper.Map<List<DeliveryOutput>>(Report);
                if (Report != null)
                {
                    totalRecord = Report.Count;
                    rtn.recordsFiltered = Report.Count;
                    rtn.data = response;
                }
                request.order= new List<Order>()
                {
                 new Order {column = "M.MessageId", dir = "desc"}
                };

                var TotalList = await _reportRepository.GetMessageDeliveryReport(request.StartDate, request.EndDate, 0, int.MaxValue, "",request.order, request.CompanyKey);
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
    }
}
