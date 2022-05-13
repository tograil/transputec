﻿using AutoMapper;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentPingStats;
using CrisesControl.Api.Application.Commands.Reports.GetIndidentMessageAck;
using CrisesControl.Api.Application.Commands.Reports.GetSOSItems;
using CrisesControl.Api.Application.Commands.Reports.ResponsesSummary;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class ReportsQuery : IReportsQuery 
    {
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
    }
}
