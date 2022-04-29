using AutoMapper;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query {
    public class CompanyParametersQuery : ICompanyParametersQuery {

        private readonly ICompanyParametersRepository _companyParametersRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyParametersQuery> _logger;

        public CompanyParametersQuery(ICompanyParametersRepository companyParametersRepository, IMapper mapper, ILogger<CompanyParametersQuery> logger)
        {
            this._companyParametersRepository=companyParametersRepository;
            this._mapper=mapper;
            this._logger=logger;
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
    }
}

