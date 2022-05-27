using AutoMapper;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Core.Compatibility;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageNoAck;
using CrisesControl.Core.Reports.SP_Response;

namespace CrisesControl.Api.Application.Query
{
    public class ReportsQuery : IReportsQuery
    {
        private readonly IReportsRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly string _timeZoneId = "GMT Standard Time";
        private readonly ILogger<ReportsQuery> _logger;

        public ReportsQuery(IReportsRepository reportRepository,
                            IMapper mapper,
                            string timeZoneId,
                            ILogger<ReportsQuery> logger)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _timeZoneId = timeZoneId;
            _logger = logger;
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
            var stats = await _reportRepository.GetIndidentMessageAck(request.MessageId, request.MessageAckStatus = 2, request.MessageSentStatus, request.RecordStart = 0, request.RecordLength, request.SearchString, "U.UserId", request.OrderDir, request.CurrentUserId, request.Filters, request.CompanyKey, request.Source);

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

        public GetCurrentIncidentStatsResponse GetCurrentIncidentStats()
        {
            return _reportRepository.GetCurrentIncidentStats(_timeZoneId);
        }

        public IncidentData GetIncidentData(int incidentActivationId, int userId, int companyId)
        {
            return _reportRepository.GetIncidentData(incidentActivationId, userId, companyId);
        }

    }
}
