using AutoMapper;
using CrisesControl.Api.Application.Commands.System.ExportTrackingData;
using CrisesControl.Api.Application.Commands.System.ViewModelLog;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Maintenance.Interfaces;
using CrisesControl.Core.Compatibility;
using CrisesControl.Core.System.Repositories;

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
