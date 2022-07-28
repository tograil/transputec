using AutoMapper;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpDept;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpLoc;
using CrisesControl.Api.Application.Commands.Lookup.GetAllTmpUser;
using CrisesControl.Api.Application.Commands.Lookup.GetIcons;
using CrisesControl.Api.Application.Commands.Lookup.GetImportTemplates;
using CrisesControl.Api.Application.Commands.Lookup.GetTempDept;
using CrisesControl.Api.Application.Commands.Lookup.GetTempLoc;
using CrisesControl.Api.Application.Commands.Lookup.GetTempUser;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Lookup.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query
{
    public class LookupQuery : ILookupQuery
    {
        private readonly ILookupRepository _lookupRepository;
        //private readonly IMapper _mapper;
        private readonly ILogger<LookupQuery> _logger;
        private readonly ICurrentUser _currentUser;
        public LookupQuery(ILookupRepository lookupRepository,  ILogger<LookupQuery> logger, ICurrentUser currentUser)
        {
            this._lookupRepository = lookupRepository;
            //this._mapper = mapper;
            this._logger = logger;
            this._currentUser = currentUser;
        }
        public async Task<GetAllTmpDeptResponse> GetAllTmpDept(GetAllTmpDeptRequest request)
        {
            try
            {
                var departments = await _lookupRepository.GetAllTmpDept();
                //var result = _mapper.Map<List<UserDepartment>>(departments);
                var response = new GetAllTmpDeptResponse();
                if (departments != null)
                {
                    response.Data = departments;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetAllTmpLocResponse> GetAllTmpLoc(GetAllTmpLocRequest request)
        {
            try
            {
                var locations = await _lookupRepository.GetAllTmpLoc();
               // var result = _mapper.Map<List<UserLocation>>(locations);
                var response = new GetAllTmpLocResponse();
                if (locations != null)
                {
                    response.Data = locations;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = locations;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetAllTmpUserResponse> GetAllTmpUser(GetAllTmpUserRequest request)
        {
            try
            {
                var tempUser = await _lookupRepository.GetAllTmpUser();
                //var result = _mapper.Map<List<Registration>>(locations);
                var response = new GetAllTmpUserResponse();
                if (tempUser != null)
                {
                    response.Data = tempUser;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = tempUser;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetIconsResponse> GetIcons(GetIconsRequest request)
        {
            try
            {
                var icons = await _lookupRepository.GetIcons(_currentUser.CompanyId);
                //var result = _mapper.Map<List<Icon>>(locations);
                var response = new GetIconsResponse();
                if (icons != null)
                {
                    response.Data = icons;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = icons;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetImportTemplatesResponse> GetImportTemplates(GetImportTemplatesRequest request)
        {
            try
            {
                var templates = await _lookupRepository.GetImportTemplates(request.Type);
                //var result = _mapper.Map<List<ImportTemplate>>(templates);
                var response = new GetImportTemplatesResponse();
                if (templates != null)
                {
                    response.Data = templates;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = templates;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTempDeptResponse> GetTempDept(GetTempDeptRequest request)
        {
            try
            {
                var dept = await _lookupRepository.GetTempDept(request.TempDeptId);
               // var result = _mapper.Map<UserDepartment>(templates);
                var response = new GetTempDeptResponse();
                if (dept != null)
                {
                    response.Data = dept;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = dept;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTempLocResponse> GetTempLoc(GetTempLocRequest request)
        {
            try
            {
                var templates = await _lookupRepository.GetTempLoc(request.TempLocationId);
                //var result = _mapper.Map<UserLocation>(templates);
                var response = new GetTempLocResponse();
                if (templates != null)
                {
                    response.Data = templates;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = templates;
                    response.Message = "No Data Found";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetTempUserResponse> GetTempUser(GetTempUserRequest request)
        {
            try
            {
                var TempUser = await _lookupRepository.GetTempUser(request.TempUserId);
                //var result = _mapper.Map<Registration>(templates);
                var response = new GetTempUserResponse();
                if (TempUser != null)
                {
                    response.Data = TempUser;
                    response.Message = "Data loaded";
                }
                else
                {
                    response.Data = TempUser;
                    response.Message = "No Data Found";
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
