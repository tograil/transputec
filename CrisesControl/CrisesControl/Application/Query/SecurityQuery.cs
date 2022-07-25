using AutoMapper;
using CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup;
using CrisesControl.Core.Security;

namespace CrisesControl.Api.Application.Query
{
    public class SecurityQuery : ISecurityQuery
    {
        private readonly ISecurityRepository _securityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SecurityQuery> _logger;
        public SecurityQuery(ISecurityRepository securityRepository, IMapper _mapper, ILogger<SecurityQuery> logger)
        {
            this._mapper = _mapper; 
            this._logger = logger;
            this._securityRepository=securityRepository;
        }
        public async Task<GetCompanySecurityGroupResponse> GetCompanySecurityGroup(GetCompanySecurityGroupRequest request)
        {
            var securities = await _securityRepository.GetCompanySecurityGroup(request.CompanyID);
            //List<GetCompanySecurityGroupResponse> response = _mapper.Map<List<CompanySecurityGroup>, List<GetCompanySecurityGroupResponse>>(securities.ToList());
            var response = _mapper.Map<List<CompanySecurityGroup>>(securities);
            var result = new GetCompanySecurityGroupResponse();
            result.Data = response;
            result.ErrorCode = "0";           
            return result;
        }
    }
}
