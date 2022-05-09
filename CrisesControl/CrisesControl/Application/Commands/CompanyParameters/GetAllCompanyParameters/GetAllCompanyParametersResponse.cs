using CrisesControl.Core.CompanyParameters;

namespace CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters {
    public class GetAllCompanyParametersResponse {
        public List<CompanyParameterItem> Data { get; set; }
        public string ErrorCode { get; set; }
    }
}
