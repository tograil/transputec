using AutoMapper;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.CompanyParameters.Repositories;

namespace CrisesControl.Api.Application.Query {
    public class CompanyParametersQuery : ICompanyParametersQuery {

        private readonly ICompanyParametersRepository _companyParamsRepository;
        private readonly IMapper _mapper;

        public CompanyParametersQuery(ICompanyParametersRepository companyParamsRepository, IMapper mapper,
           ILogger<BillingQuery> logger) {
            _mapper = mapper;
            _companyParamsRepository = companyParamsRepository;
        }

        public async Task<GetAllCompanyParametersResponse> GetAllCompanyParameters(GetAllCompanyParametersRequest request) {
            var msgresponse = await _companyParamsRepository.GetAllCompanyParameters(request.CompanyId);
            var response = _mapper.Map<List<CompanyParameterItem>>(msgresponse);
            var result = new GetAllCompanyParametersResponse();
            result.Data = response;
            result.ErrorCode = "0";
            return result;
        }
    }
}
