using CrisesControl.Api.Application.Commands.Companies.CheckCompany;
using CrisesControl.Api.Application.Commands.Companies.DeleteCompany;
using CrisesControl.Api.Application.Commands.Companies.GetCommsMethod;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.Commands.Companies.GetSite;
using CrisesControl.Api.Application.Commands.Companies.ViewCompany;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query;

public interface ICompanyQuery
{
    public Task<IEnumerable<CompanyInfo>> GetCompanyList(int? status, string? companyProfile);
    public Task<GetCompanyResponse> GetCompany(GetCompanyRequest request, CancellationToken cancellationToken);
    public Task<GetCommsMethodResponse> GetCommsMethod(CancellationToken cancellationToken);
    Task<CheckCompanyResponse> CheckCompany(CheckCompanyRequest request);
    Task<DeleteCompanyResponse> DeleteCompany(DeleteCompanyRequest request);
    Task<ViewCompanyResponse> ViewCompany(ViewCompanyRequest request);
    Task<GetSiteResponse> GetSite(GetSiteRequest request);
}