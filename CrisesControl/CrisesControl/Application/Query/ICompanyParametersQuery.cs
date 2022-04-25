using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;

namespace CrisesControl.Api.Application.Query {
    public interface ICompanyParametersQuery {
        public Task<GetAllCompanyParametersResponse> GetAllCompanyParameters(GetAllCompanyParametersRequest request);
    }
}