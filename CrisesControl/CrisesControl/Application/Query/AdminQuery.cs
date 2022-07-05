using AutoMapper;
using CrisesControl.Api.Application.Commands.Administrator.AddLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.AddLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.DeleteLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.DumpReport;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetAllLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetCompanyPackageFeatures;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.GetLibIncidentType;
using CrisesControl.Api.Application.Commands.Administrator.GetReport;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncident;
using CrisesControl.Api.Application.Commands.Administrator.UpdateLibIncidentType;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.SharedKernel.Utils;
using System.Data;

namespace CrisesControl.Api.Application.Query
{
    public class AdminQuery: IAdminQuery
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AdminQuery> _logger;
        private readonly IAdminRepository _adminRepository;
        private readonly ICurrentUser _currentUser;
        public AdminQuery(IMapper mapper, ILogger<AdminQuery> logger, IAdminRepository administratorRepository, ICurrentUser currentUser)
        {
            this._logger=logger;
            this._mapper=mapper;
            this._adminRepository=administratorRepository;
            this._currentUser = currentUser;
        }

        public async Task<AddLibIncidentResponse> AddLibIncident(AddLibIncidentRequest request)
        {
            var IsLibIncidentExist = await _adminRepository.GetLibIncidentByName(request.Name);
            var response = new AddLibIncidentResponse();
            if (IsLibIncidentExist != null && IsLibIncidentExist.LibIncidentId == request.LibIncidentId)
            {
                IsLibIncidentExist.Name = request.Name;
                IsLibIncidentExist.Description = request.Description;
                IsLibIncidentExist.LibIncidentTypeId = request.LibIncidentTypeId;
                IsLibIncidentExist.LibIncodentIcon = request.LibIncidentIcon;
                IsLibIncidentExist.Severity = request.Severity;
                IsLibIncidentExist.Status = request.Status;
                IsLibIncidentExist.UpdatedBy = _currentUser.UserId;
                IsLibIncidentExist.UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                var LibIncidentId = await _adminRepository.UpdateLibIncident(IsLibIncidentExist);
                response.LibIncidentId = LibIncidentId;
               
            }
            else if (IsLibIncidentExist == null && request.LibIncidentId == 0)
            {
                LibIncident newLibIncident = new LibIncident()
                {
                    Name = request.Name,
                    Description = request.Description,
                    LibIncidentTypeId = request.LibIncidentTypeId,
                    LibIncodentIcon = request.LibIncidentIcon,
                    Severity = request.Severity,
                    Status = 1,
                    CreatedBy = _currentUser.UserId,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = _currentUser.UserId,
                    UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone)
                };
               var newLibIncidentId= await _adminRepository.AddLibIncident(newLibIncident);
                response.LibIncidentId = newLibIncidentId;

            }
            return response;
        }

        public async Task<DeleteLibIncidentResponse> DeleteLibIncident(DeleteLibIncidentRequest request)
        {
            var DeleteLibIncident = await _adminRepository.GetLibIncidentById(request.LibIncidentId);
            var response = new DeleteLibIncidentResponse();
            if (DeleteLibIncident != null)
            {
                 var isDeleted= await  _adminRepository.DeleteLibIncident(DeleteLibIncident);
                if(isDeleted)
                response.Deleted = isDeleted;
                response.Message = "Data Deleted.";
            }
            else
            {
                response.Deleted = false;
                response.Message = "No record found.";
            }
            return response;
        }

        public async Task<DumpReportResponse> DumpReport(DumpReportRequest request)
        {
            string rFilePath = string.Empty;
            string rFileName = string.Empty;
            DataTable dataTable =await  _adminRepository.GetReportData(request.ReportID, request.ParamList, rFilePath, rFileName);
            var response = new DumpReportResponse();
            if (request.DownloadFile)
            {
                var data = await _adminRepository.ToCSVHighPerformance(dataTable, true, ",");
                using (StreamWriter SW = new StreamWriter(rFilePath, false))
                {
                    SW.Write(data);
                }
                response.result = rFileName;
            }
            else
            {
                DataSet dsData = new DataSet();
                dsData.Tables.Add(dataTable);
                response.result= dsData.Tables[0].ToString();
            }
            return response;
        }

        public async Task<GetAllLibIncidentResponse> GetAllLibIncident(GetAllLibIncidentRequest request)
        {
            var libIncidents = await _adminRepository.GetAllLibIncident();
            var response = _mapper.Map<List<LibIncident>>(libIncidents);
            var result = new GetAllLibIncidentResponse();
            result.data = response;
            result.Message = "Data loaded Successfully";
            return result;
        }

        public async Task<GetReportResponse> GetReport(GetReportRequest request)
        {
            try
            {
                var libIncidents = await _adminRepository.GetReportList(request.ReportId);
                var result = _mapper.Map<AdminReport>(libIncidents);
                var response = new GetReportResponse();
                if (result!=null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }
                
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetLibIncidentResponse> GetLibIncident(GetLibIncidentRequest request)
        {
            try
            {
                var libIncidents = await _adminRepository.GetLibIncident(request.LibIncidentId);
                var result = _mapper.Map<AdminLibIncident>(libIncidents);
                var response = new GetLibIncidentResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateLibIncidentResponse> UpdateLibIncident(UpdateLibIncidentRequest request)
        {
            try
            {
                var IsLibIncidentExist = await _adminRepository.GetLibIncidentByName(request.Name);
                var result = _mapper.Map<AdminReport>(IsLibIncidentExist);
                var response = new UpdateLibIncidentResponse();


                if (result != null && IsLibIncidentExist.LibIncidentId == request.LibIncidentId)
                {
                    IsLibIncidentExist.Name = request.Name;
                    IsLibIncidentExist.Description = request.Description;
                    IsLibIncidentExist.LibIncidentTypeId = request.LibIncidentTypeId;
                    IsLibIncidentExist.LibIncodentIcon = request.LibIncidentIcon;
                    IsLibIncidentExist.Severity = request.Severity;
                    IsLibIncidentExist.Status = request.Status;
                    IsLibIncidentExist.UpdatedBy = _currentUser.UserId;
                    IsLibIncidentExist.UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone);
                    var LibIncidentID = await _adminRepository.UpdateLibIncident(IsLibIncidentExist);
                    response.LibIncidentId = LibIncidentID;
                    
                }
                else if (IsLibIncidentExist == null && request.LibIncidentId == 0)
                {
                    LibIncident newLibIncident = new LibIncident()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        LibIncidentTypeId = request.LibIncidentTypeId,
                        LibIncodentIcon = request.LibIncidentIcon,
                        Severity = request.Severity,
                        Status = 1,
                        CreatedBy = _currentUser.UserId,
                        CreatedOn = DateTime.Now,
                        UpdatedBy = _currentUser.UserId,
                        UpdatedOn = DateTime.Now.GetDateTimeOffset(_currentUser.TimeZone)
                    };
                 
                    var newLibIncidentID = await _adminRepository.AddLibIncident(newLibIncident);
                    response.LibIncidentId = newLibIncidentID;
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetAllLibIncidentTypeResponse> GetAllLibIncidentType(GetAllLibIncidentTypeRequest request)
        {
            try
            {
                var libIncidents = await _adminRepository.GetAllLibIncidentType();
                var result = _mapper.Map<List<LibIncidentType>>(libIncidents);
                var response = new GetAllLibIncidentTypeResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<UpdateLibIncidentTypeResponse> UpdateLibIncidentType(UpdateLibIncidentTypeRequest request)
        {
            try
            {
                var LibIncidentTypeExist = await _adminRepository.GetLibIncidentTypeByName(request.Name);
                var response = new UpdateLibIncidentTypeResponse();
                if (LibIncidentTypeExist != null && request.LibIncidentTypeId == LibIncidentTypeExist.LibIncidentTypeId)
                {
                    LibIncidentTypeExist.Name = request.Name;
                    var LibTypeId = await _adminRepository.UpdateLibIncidentType(LibIncidentTypeExist);
                }
                else if (LibIncidentTypeExist == null && request.LibIncidentTypeId == 0)
                {
                    LibIncidentType newLibIncidentType = new LibIncidentType()
                    {
                        Name = request.Name
                    };
                    var LibTypeId = await _adminRepository.AddLibIncidentType(newLibIncidentType);
                    response.LibIncidentTypeId = LibTypeId;
                }
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetLibIncidentTypeResponse> GetLibIncidentType(GetLibIncidentTypeRequest request)
        {
            try
            {
                var libIncidentType = await _adminRepository.GetLibIncidentType(request.LibIncidentTypeId);
                var result = _mapper.Map<List<LibIncidentType>>(libIncidentType);
                var response = new GetLibIncidentTypeResponse();
                if (result != null)
                {
                    response.Data = result;
                    response.Message = "Data loaded Successfully";
                }
                else
                {
                    response.Data = result;
                    response.Message = "No record found.";
                }

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AddLibIncidentTypeResponse> AddLibIncidentType(AddLibIncidentTypeRequest request)
        {
            try
            {
                var LibIncidentTypeExist = await _adminRepository.GetLibIncidentTypeByName(request.Name);
                
                var response = new AddLibIncidentTypeResponse();

                if (LibIncidentTypeExist != null && request.LibIncidentTypeId == LibIncidentTypeExist.LibIncidentTypeId)
                {
                    LibIncidentTypeExist.Name = request.Name;
                    var libIncidentTypeId = await _adminRepository.UpdateLibIncidentType(LibIncidentTypeExist);
                    response.LibInclidentTypeId = libIncidentTypeId;
                    response.Message = "Data loaded Successfully";

                }
                else if (LibIncidentTypeExist == null && request.LibIncidentTypeId == 0)
                {
                    LibIncidentType newLibIncidentType = new LibIncidentType()
                    {
                        Name = request.Name
                    };
                    var newLibIncidentTypeId = await _adminRepository.AddLibIncidentType(newLibIncidentType);
                    response.LibInclidentTypeId = newLibIncidentTypeId;
                    response.Message = "Data loaded Successfully";
                }
            

                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<DeleteLibIncidentTypeResponse> DeleteLibIncidentType(DeleteLibIncidentTypeRequest request)
        {
            try
            {
                var DeleteLibIncident = await _adminRepository.GetLibIncidentTypeById(request.LibIncidentTypeId);
                var response = new DeleteLibIncidentTypeResponse();
                if (DeleteLibIncident != null)
                {
                    var isDeleted = await _adminRepository.DeleteLibIncidentType(DeleteLibIncident);
                    if (isDeleted) { 
                        response.Deleted = isDeleted;
                    response.Message = "Data Deleted.";
                    }
                }
                else
                {
                    response.Deleted = false;
                    response.Message = "No record found.";
                }
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<GetCompanyPackageFeaturesResponse> GetCompanyPackageFeatures(GetCompanyPackageFeaturesRequest request)
        {
            try
            {
                var modules = await _adminRepository.GetCompanyModules(_currentUser.CompanyId);
                var feature = await _adminRepository.GetCompanyPackageFeatures(_currentUser.CompanyId);
                var result = _mapper.Map<List<CompanyPackageFeatureList>>(feature);
                var resultModules = _mapper.Map<List<CompanyPackageFeatureList>>(modules);
                var response = new GetCompanyPackageFeaturesResponse();
                if (result != null || resultModules!=null)
                {
                    response.Feature = result;
                    response.Modules = resultModules;
                }
                else
                {
                    response.Feature = result;
                    response.Modules = resultModules;
                    response.Message = "No record found.";
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
