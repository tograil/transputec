using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;

namespace CrisesControl.Api.Application.Query
{
    public interface ICompanyParametersQuery
    {
        public Task<GetCascadingResponse> GetCascading(GetCascadingRequest request);
        public Task<GetCompanyFTPResponse> GetCompanyFTP(GetCompanyFTPRequest request);
    }
}
