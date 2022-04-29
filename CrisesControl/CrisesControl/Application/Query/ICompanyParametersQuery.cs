using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;

namespace CrisesControl.Api.Application.Query
{
    public interface ICompanyParametersQuery
    {
        public Task<GetCascadingResponse> GetCascading(GetCascadingRequest request);
        public Task<GetCompanyFTPResponse> GetCompanyFTP(GetCompanyFTPRequest request);
        public Task<GetAllCompanyParametersResponse> GetAllCompanyParameters(GetAllCompanyParametersRequest request);
    }
}
