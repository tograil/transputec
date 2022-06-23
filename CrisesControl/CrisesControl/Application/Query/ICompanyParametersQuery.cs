using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveCompanyFTP;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Companies;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveParameter;
using CrisesControl.Api.Application.Commands.CompanyParameters.DeleteCascading;

namespace CrisesControl.Api.Application.Query
{
    public interface ICompanyParametersQuery
    {
        public Task<GetCascadingResponse> GetCascading(GetCascadingRequest request);
        public Task<GetCompanyFTPResponse> GetCompanyFTP(GetCompanyFTPRequest request);
        public Task<GetAllCompanyParametersResponse> GetAllCompanyParameters(GetAllCompanyParametersRequest request);
        Task<SaveCompanyFTPResponse> SaveCompanyFTP(SaveCompanyFTPRequest request);
        Task<SaveCascadingResponse> SaveCascading(SaveCascadingRequest request);
        Task<SaveParameterResponse> SaveParameter(SaveParameterRequest request);
        Task<DeleteCascadingResponse> DeleteCascading(DeleteCascadingRequest request);
    }
}
