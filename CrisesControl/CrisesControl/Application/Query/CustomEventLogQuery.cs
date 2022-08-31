using AutoMapper;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLog;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetEventLogHeader;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetLogs;
using CrisesControl.Api.Application.Commands.CustomEventLog.GetMessageLog;
using CrisesControl.Core.CustomEventLog.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class CustomEventLogQuery : ICustomEventLogQuery
    {
        private readonly ICustomEventLogRepository _customEventLogRepository;
        private readonly IMapper _mapper;
        public CustomEventLogQuery( ICustomEventLogRepository customEventLogRepository, IMapper mapper)
        {
            _customEventLogRepository = customEventLogRepository;
            _mapper = mapper;
        }
        public async Task<GetEventLogResponse> GetEventLog(GetEventLogRequest request)
        {
            try
            {
                var result = await _customEventLogRepository.GetEventLog(request.EventLogId, request.EventLogHeaderId, request.CompanyId, request.UserId);
                var response = _mapper.Map<GetEventLogResponse>(result);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetEventLogHeaderResponse> GetEventLogHeader(GetEventLogHeaderRequest request)
        {
            try
            {
                var result = await _customEventLogRepository.GetEventLogHeader(request.ActiveIncidentId, request.EventLogHeaderId, request.CompanyId, request.UserId);
                var response = _mapper.Map<GetEventLogHeaderResponse>(result);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetLogsResponse> GetLogs(GetLogsRequest request)
        {
            try
            {
                var result = await _customEventLogRepository.GetLogs(request.ActiveIncidentId, request.EventLogHeaderId, request.CompanyId, request.UserId);
                var response = new GetLogsResponse();
                response.Data = result;
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetMessageLogResponse> GetMessageLog(GetMessageLogRequest request)
        {
            try
            {
                var result = await _customEventLogRepository.GetMessageLog(request.EventLogId, request.CompanyId, request.UserId);
                var response = new GetMessageLogResponse();
                response.Data = result;
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
