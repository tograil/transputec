using AutoMapper;
using CrisesControl.Api.Application.Commands.Support.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.Support.GetIncidentData;
using CrisesControl.Api.Application.Commands.Support.GetIncidentMessageAck;
using CrisesControl.Api.Application.Commands.Support.GetIncidentReportDetails;
using CrisesControl.Api.Application.Commands.Support.GetIncidentStats;
using CrisesControl.Api.Application.Commands.Support.GetUser;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Core.Support.Repositories;

namespace CrisesControl.Api.Application.Query
{
    public class SupportQuery : ISupportQuery
    {
        private readonly ISupportRepository _supportRepository;
        private readonly IReportsRepository _reportsRepository;
        private readonly IActiveIncidentRepository _activeIncidentRepository;
        private readonly IMapper _mapper;

        public SupportQuery(ISupportRepository supportRepository, IReportsRepository reportsRepository, IMapper mapper, IActiveIncidentRepository activeIncidentRepository)
        {
            _supportRepository = supportRepository;
            _reportsRepository = reportsRepository;
            _activeIncidentRepository = activeIncidentRepository;
            _mapper = mapper;
        }
        public async Task<ActiveIncidentTasksResponse> ActiveIncidentTasks(ActiveIncidentTasksRequest request)
        {
            try
            {
                var response = new ActiveIncidentTasksResponse();
                response.Task = await _activeIncidentRepository.GetActiveIncidentWorkflow(request.ActiveIncidentId);
                response.IncidentTaskDetails = await _activeIncidentRepository.GetActiveIncidentTasks(request.ActiveIncidentId, request.ActiveIncidentTaskId, request.CompanyId, request.Single);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetIncidentDataResponse> GetIncidentData(GetIncidentDataRequest request)
        {
            try
            {
                var result = await _supportRepository.GetIncidentData(request.IncidentActivationId, request.CompanyId);
                var response = _mapper.Map<GetIncidentDataResponse>(result);
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetIncidentMessageAckResponse> GetIncidentMessageAck(GetIncidentMessageAckRequest request)
        {
            try
            {
                var response = new GetIncidentMessageAckResponse();
                response.Data = await _reportsRepository.GetIndidentMessageAck(request.MessageId, request.MessageAckStatus, 
                    request.MessageSentStatus, request.RecordStart, request.RecordLength, request.search, 
                    request.OrderBy, request.OrderDir, request.draw, request.Filters, request.CompanyKey,
                    request.Source);
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetIncidentReportDetailsResponse> GetIncidentReportDetails(GetIncidentReportDetailsRequest request)
        {
            try
            {
                var response = new GetIncidentReportDetailsResponse();
                response.Data = await _reportsRepository.GetIndidentReportDetails(request.IncidentActivationId, request.CompanyId, request.UserId);
                return response;
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
                var result = await _reportsRepository.GetIncidentStats(request.IncidentActivationId, request.OutUserCompanyId);
                var response = _mapper.Map<GetIncidentStatsResponse>(result);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            try
            {
                var result = await _supportRepository.GetUser(request.UserId);
                var response = _mapper.Map<GetUserResponse>(result);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
