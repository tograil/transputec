using AutoMapper;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;

namespace CrisesControl.Api.Application.Query {
    public class ReportsQuery : IReportsQuery {
        private readonly IReportsRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BillingQuery> _logger;

        public ReportsQuery(IReportsRepository reportRepository, IMapper mapper,
            ILogger<BillingQuery> logger) {
            _mapper = mapper;
            _reportRepository = reportRepository;
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

    }
}
