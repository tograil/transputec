using AutoMapper;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveCompanyFTP;
using CrisesControl.Api.Application.Helpers;

namespace CrisesControl.Api.Application.Query {
    public class CompanyParametersQuery : ICompanyParametersQuery {

        private readonly ICompanyParametersRepository _companyParametersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyParametersQuery> _logger;
        private readonly ICurrentUser _currentUser;

        public CompanyParametersQuery(ICompanyParametersRepository companyParametersRepository, IMapper mapper, ILogger<CompanyParametersQuery> logger, ICurrentUser currentUser)
        {
            this._companyParametersRepository=companyParametersRepository;
            this._mapper=mapper;
            this._logger=logger;
            this._currentUser = currentUser;
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
            var response = _mapper.Map<int>(ftp);
            var result = new SaveCompanyFTPResponse();
            if (ftp>0)
            {
                result.ResultId = response;
                result.Message = "CompanyFTP has been found";
            }
           
            return result;
        }
    }
}

