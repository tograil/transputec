using AutoMapper;
using CrisesControl.Api.Application.Commands.Reports.GetIncidentMessageAck;
using CrisesControl.Api.Application.Commands.Support.ActiveIncidentTasks;
using CrisesControl.Api.Application.Commands.Support.GetIncidentData;
using CrisesControl.Api.Application.Commands.Support.GetIncidentMessageAck;
using CrisesControl.Api.Application.Commands.Support.GetIncidentReportDetails;
using CrisesControl.Api.Application.Commands.Support.GetIncidentStats;
using CrisesControl.Api.Application.Commands.Support.GetUser;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Compatibility;
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
        private readonly IPaging _paging;

        public SupportQuery(ISupportRepository supportRepository, IReportsRepository reportsRepository, IMapper mapper, 
            IActiveIncidentRepository activeIncidentRepository,
            IPaging paging)
        {
            _supportRepository = supportRepository;
            _reportsRepository = reportsRepository;
            _activeIncidentRepository = activeIncidentRepository;
            _mapper = mapper;
            _paging = paging;
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

        public async Task<Commands.Support.GetIncidentMessageAck.GetIncidentMessageAckResponse> GetIncidentMessageAck(Commands.Support.GetIncidentMessageAck.GetIncidentMessageAckRequest request)
        {
            try
            {
                var ackresult = await _reportsRepository.GetIncidentMessageAck(request.MessageId, request.MessageAckStatus, request.MessageSentStatus, request.Source,
                    _paging.Start, _paging.Length, _paging.Search, _paging.OrderBy, _paging.Dir, _paging.Filters, _paging.UniqueKey);

                var response = _mapper.Map<DataTablePaging>(ackresult);
                var result = new Commands.Support.GetIncidentMessageAck.GetIncidentMessageAckResponse();
                result.Data = response;
                result.ErrorCode = System.Net.HttpStatusCode.OK;
                return result;
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
                response.Data = await _reportsRepository.GetIncidentReportDetails(request.IncidentActivationId, request.CompanyId, request.UserId);
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
