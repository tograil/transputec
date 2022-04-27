using CrisesControl.Core.Security;

namespace CrisesControl.Api.Application.Commands.Security.GetCompanySecurityGroup
{
    public class GetCompanySecurityGroupResponse
    {
        public List<CompanySecurityGroup> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
