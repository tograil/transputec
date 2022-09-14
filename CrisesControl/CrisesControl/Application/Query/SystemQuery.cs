using AutoMapper;
using CrisesControl.Api.Application.Commands.System.ApiStatus;
using CrisesControl.Api.Application.Commands.System.CleanLoadTestResult;
using CrisesControl.Api.Application.Commands.System.CompanyStatsAdmin;
using CrisesControl.Api.Application.Commands.System.DownloadExportFile;
using CrisesControl.Api.Application.Commands.System.ExportCompanyData;
using CrisesControl.Api.Application.Commands.System.ExportTrackingData;
using CrisesControl.Api.Application.Commands.System.GetAuditLogsByRecordId;
using CrisesControl.Api.Application.Commands.System.PushCMLog;
using CrisesControl.Api.Application.Commands.System.PushTwilioLog;
using CrisesControl.Api.Application.Commands.System.TwilioLogDump;
using CrisesControl.Api.Application.Commands.System.ViewErrorLog;
using CrisesControl.Api.Application.Commands.System.ViewModelLog;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.System;
using CrisesControl.Core.System.Repositories;
using CrisesControl.SharedKernel.Utils;

namespace CrisesControl.Api.Application.Query
{
    public class SystemQuery : ISystemQuery
    {
        private readonly ISystemRepository _systemRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SystemQuery> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IPaging _paging;
        public SystemQuery(ISystemRepository systemRepository, ICurrentUser currentUser, ILogger<SystemQuery> logger, IMapper mapper, IPaging paging)
        {
            this._currentUser = currentUser;
            this._logger = logger;
            this._systemRepository = systemRepository;
            this._mapper = mapper;
            this._paging = paging;
        }

        public async Task<ApiStatusResponse> ApiStatus(ApiStatusRequest request)
        {
            try
            {
                var status = await _systemRepository.ApiStatus();
                var result = _mapper.Map<HttpResponseMessage>(status);
                var response = new ApiStatusResponse();
                if (result != null)
                {
                    response.HttpResponse = result;

                }
                else
                {
                    response.HttpResponse = result;
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CleanLoadTestResultResponse> CleanLoadTestResult(CleanLoadTestResultRequest request)
        {
            try
            {
                await _systemRepository.CleanLoadTestResult();
                var response = new CleanLoadTestResultResponse();
                response.Result = true;
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompanyStatsAdminResponse> CompanyStatsAdmin(CompanyStatsAdminRequest request)
        {
            try
            {
                var libIncidents = await _systemRepository.CompanyStatsAdmin( _currentUser.CompanyId);
                var result = _mapper.Map<HttpResponseMessage>(libIncidents);
                var response = new CompanyStatsAdminResponse();
                if (result != null)
                {
                    response.HttpResponse = result;

                }
                else
                {
                    response.HttpResponse = result;
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DownloadExportFileResponse> DownloadExportFile(DownloadExportFileRequest request)
        {
            try
            {
                var libIncidents = await _systemRepository.DownloadExportFile(request.CompanyId,request.FileName);
                var result = _mapper.Map<HttpResponseMessage>(libIncidents);
                var response = new DownloadExportFileResponse();
                if (result != null)
                {
                    response.HttpResponse = result;

                }
                else
                {
                    response.HttpResponse = result;
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ExportCompanyDataResponse> ExportCompanyData(ExportCompanyDataRequest request)
        {
            try
            {
                var libIncidents = await _systemRepository.ExportCompanyData(_currentUser.CompanyId, request.Entity,_currentUser.UserId, request.ShowDeleted);
                var result = _mapper.Map<string>(libIncidents);
                var response = new ExportCompanyDataResponse();
                if (result != null)
                {
                    response.CompanyData = result;

                }
                else
                {
                    response.CompanyData = "No record found";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ExportTrackingDataResponse> ExportTrackingData(ExportTrackingDataRequest request)
        {
            try
            {
                var libIncidents = await _systemRepository.ExportTrackingData(request.TrackMeID,request.UserDeviceID,request.StartDate,request.EndDate, _currentUser.CompanyId);
                var result = _mapper.Map<string>(libIncidents);
                var response = new ExportTrackingDataResponse();
                if (result != null)
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

        public async Task<GetAuditLogsByRecordIdResponse> GetAuditLogsByRecordId(GetAuditLogsByRecordIdRequest request)
        {
            try
            {
                var auditHelps = await _systemRepository.GetAuditLogsByRecordId(request.TableName, request.RecordId, request.IsThisWeek, request.IsThisMonth,request.IsLastMonth,request.StartDate,request.EndDate,request.LimitResult, _currentUser.CompanyId);
                var result = _mapper.Map<List<AuditHelp>>(auditHelps);
                var response = new GetAuditLogsByRecordIdResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded";

                }
                else
                {
                    response.Data = null;
                    response.Message = "No record Found";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PushCMLogResponse> PushCMLog(PushCMLogRequest request)
        {
            try
            {
                var cmLog = await _systemRepository.PushCMLog(request.MessageType.ToDbString(),request.Sid);
                var result = _mapper.Map<bool>(cmLog);
                var response = new PushCMLogResponse();
                if (result)
                {
                    response.Result = result;
                    response.Message = "CM Log Pushed";

                }
                else
                {
                    response.Result = false;
                    response.Message = "No record Found";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PushTwilioLogResponse> PushTwilioLog(PushTwilioLogRequest request)
        {
            try
            {
                var cmLog = await _systemRepository.PushTwilioLog(request.MessageType.ToDbString(), request.Sid);
                var result = _mapper.Map<bool>(cmLog);
                var response = new PushTwilioLogResponse();
                if (result)
                {
                    response.Result = result;
                    response.Message = "Twilio Log Pushed";

                }
                else
                {
                    response.Result = false;
                    response.Message = "No record Found";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TwilioLogDumpResponse> TwilioLogDump(TwilioLogDumpRequest request)
        {
            try
            {
                var cmLog = await _systemRepository.TwilioLogDump(request.LogType.ToLGString(),request.Calls,request.Texts,request.Recordings);
                var result = _mapper.Map<bool>(cmLog);
                var response = new TwilioLogDumpResponse();
                if (result)
                {
                    response.Result = result;
                    response.Message = "Twilio Log Dumped Response";

                }
                else
                {
                    response.Result = false;
                    response.Message = "No record Found";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ViewErrorLogResponse> ViewErrorLog(ViewErrorLogRequest request)
        {
            try
            {
                var RecordStart = _paging.PageNumber == 0 ? 0 : _paging.PageNumber;
                var RecordLength = _paging.PageSize == 0 ? int.MaxValue : _paging.PageSize;
                var SearchString = (request.search != null) ? request.search.Value : "";
              
                string OrderDir = request.order != null ? request.order.FirstOrDefault().Dir : "desc";

                int totalRecord = 0;
                DataTablePaging rtn = new DataTablePaging();
                rtn.Draw = request.draw;

                var result = await _systemRepository.GetErrorLog(request.StartDate, request.EndDate, RecordStart, RecordLength, SearchString, _paging.OrderBy, OrderDir);

                if (result != null)
                {
                    totalRecord = result.Count;
                    rtn.RecordsFiltered = result.Count;
                    rtn.Data = result;
                }

                var TotalList = await _systemRepository.GetErrorLog(request.StartDate, request.EndDate, RecordStart, int.MaxValue, "", "Id", "asc");

                if (TotalList != null)
                {
                    totalRecord = TotalList.Count;
                }

                rtn.RecordsTotal = totalRecord;

                var result1 = _mapper.Map<DataTablePaging>(rtn);
                var response = new ViewErrorLogResponse();
                if (result != null)
                {
                    response.Data = rtn;

                }
                else
                {
                    response.Data = null;
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ViewModelLogResponse> ViewModelLog(ViewModelLogRequest request)
        {
            try
            {
                string OrderDir = request.order != null ? request.order.FirstOrDefault().Dir : "desc";
                var SearchString = (request.search != null) ? request.search.Value : "";
                int totalRecord = 0;
                DataTablePaging rtn = new DataTablePaging();
                rtn.Draw = request.draw;

                var log = await _systemRepository.GetModelLog(request.StartDate, request.EndDate, _paging.PageNumber, _paging.PageSize, SearchString, _paging.OrderBy, OrderDir); ;
               

                if (log != null)
                {
                    totalRecord = log.Count();
                    rtn.RecordsFiltered = log.Count();
                    rtn.Data = log;
                }
                var TotalList = await _systemRepository.GetModelLog(request.StartDate, request.EndDate, _paging.PageNumber, int.MaxValue, "", "ModelMappingLogId", "asc");

                if (TotalList != null)
                {
                    totalRecord = TotalList.Count();
                }

                rtn.RecordsTotal = totalRecord;
                var result = _mapper.Map<DataTablePaging>(rtn);
                var response = new ViewModelLogResponse();
                if (result != null)
                {
                    response.Data = rtn;

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
    }
}
