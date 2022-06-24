using AutoMapper;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveCompanyFTP;
using CrisesControl.Api.Application.Helpers;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveParameter;
using CrisesControl.Core.Incidents.Repositories;
using CrisesControl.Api.Application.Commands.CompanyParameters.DeleteCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.SavePriority;

namespace CrisesControl.Api.Application.Query {
    public class CompanyParametersQuery : ICompanyParametersQuery {

        private readonly ICompanyParametersRepository _companyParametersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyParametersQuery> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IIncidentRepository _incidentRepository;

        public CompanyParametersQuery(ICompanyParametersRepository companyParametersRepository, IMapper mapper, 
            ILogger<CompanyParametersQuery> logger, ICurrentUser currentUser, IIncidentRepository incidentRepository)
        {
            this._companyParametersRepository=companyParametersRepository;
            this._mapper=mapper;
            this._logger=logger;
            this._currentUser = currentUser;
            this._incidentRepository = incidentRepository;
        }
        public async Task<GetCascadingResponse> GetCascading(GetCascadingRequest request)
        {
            var cascades = await _companyParametersRepository.GetCascading(request.PlanID, request.PlanType, request.CompanyID);
            var response = _mapper.Map<List<CascadingPlanReturn>>(cascades);
            var result = new GetCascadingResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<GetCompanyFTPResponse> GetCompanyFTP(GetCompanyFTPRequest request)
        {
            var company = await _companyParametersRepository.GetCompanyFTP( request.CompanyID);
            var response = _mapper.Map<List<CompanyFtp>>(company);
            var result = new GetCompanyFTPResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }

        public async Task<GetAllCompanyParametersResponse> GetAllCompanyParameters(GetAllCompanyParametersRequest request) {
            var msgresponse = await _companyParametersRepository.GetAllCompanyParameters(request.CompanyId);
            var response = _mapper.Map<List<CompanyParameterItem>>(msgresponse);
            var result = new GetAllCompanyParametersResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }
        public async Task<SaveCompanyFTPResponse> SaveCompanyFTP(SaveCompanyFTPRequest request)
        {
            var ftp = await _companyParametersRepository.SaveCompanyFTP(_currentUser.CompanyId,request.HostName,request.UserName,request.SecurityKey,request.Protocol,request.Port,request.RemotePath,request.LogonType,request.DeleteSourceFile,request.SHAFingerPrint);
            var response = _mapper.Map<Result>(ftp);
            var result = new SaveCompanyFTPResponse();
            if (ftp!=null)
            {
                result.ResultId = response;
                result.Message = "CompanyFTP has been added";
            }
           
            return result;
        }

        public async Task<SaveCascadingResponse> SaveCascading(SaveCascadingRequest request)
        {
            var ftp = await _companyParametersRepository.SaveCascading(request.PlanID, request.PlanName,request.PlanType,request.LaunchSOS, request.LaunchSOSInterval, request.CommsMethod, _currentUser.CompanyId);
            var response = _mapper.Map<bool>(ftp);
            var result = new SaveCascadingResponse();
            if (ftp)
            {
                result.Result = response;
                result.Message = "Cascading added";
            }
            else
            {
                result.Result = response;
                result.Message = "Cascading not added";
            }

            return result;
        }
        public async Task<SaveParameterResponse> SaveParameter(SaveParameterRequest request)
        {
            var response = new SaveParameterResponse();
            if (request.Parameters!=null) { 
            foreach (Parameter param in request.Parameters)
            {
               await _companyParametersRepository.SaveParameter(param.ParameterId, param.ParameterName, param.ParameterValue, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);

                if (param.ParameterName == "SOS_ENABLED" && param.ParameterValue == "true")
                {

                   await _incidentRepository.CheckSOSIncident(_currentUser.CompanyId, _currentUser.UserId, _currentUser.TimeZone);
                }

                if (param.ParameterName == "ALLOW_CHANNEL_PRIORITY" || param.ParameterName == "ALLOW_CHANGE_PRIORITY_USER")
                {
                    
                  await  _companyParametersRepository.UpdateCascadingAsync(_currentUser.CompanyId);
                }

                if (param.ParameterName == "ALLOW_OFF_DUTY" && param.ParameterValue == "false")
                {
                   
                   await _companyParametersRepository.UpdateOffDuty(_currentUser.CompanyId);
                }
            }


              await _companyParametersRepository.SetSSOParameters(_currentUser.CompanyId);
              response.Message = "Data Added";
            }
            else
            {
                response.Message = "Data Not added";
            }
            return response;





        }

        public async Task<DeleteCascadingResponse> DeleteCascading(DeleteCascadingRequest request)
        {
            var ftp = await _companyParametersRepository.DeleteCascading(request.PlanID, request.CompanyId, _currentUser.UserId);
            var response = _mapper.Map<bool>(ftp);
            var result = new DeleteCascadingResponse();
            if (ftp)
            {
                result.Result = response;
                result.Message = "Deleted";
            }
            else
            {
                result.Result = response;
                result.Message = "Data Not found";
            }

            return result;
        }

        public async Task<SavePriorityResponse> SavePriority(SavePriorityRequest request)
        {
            try
            {
                var priority = await _companyParametersRepository.SavePriority(request.ParamName, request.EnableSetting, request.CommsMethod, request.PingPriority, request.IncidentPriority, request.IncidentSeverity,
                        request.Type, _currentUser.UserId, _currentUser.CompanyId, _currentUser.TimeZone);
                var result = _mapper.Map<bool>(priority);
                var  response = new SavePriorityResponse();
                if (priority)
                {
                    response.Result = result;
                    response.Message = "Priority Saved";
                }
                else
                {
                    response.Result = result;
                    response.Message = "Priority not added";
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

