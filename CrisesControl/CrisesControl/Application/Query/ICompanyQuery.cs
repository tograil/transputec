using CrisesControl.Api.Application.Commands.Companies.GetCommsMethod;
using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.ViewModels.Company;
using CrisesControl.Core.Models;

namespace CrisesControl.Api.Application.Query;

public interface ICompanyQuery
{
    public Task<IEnumerable<CompanyInfo>> GetCompanyList(int? status, string? companyProfile);
    public Task<GetCompanyResponse> GetCompany(GetCompanyRequest request, CancellationToken cancellationToken);
    Task<GetCommsMethodResponse> GetCommsMethod(CancellationToken cancellationToken);
}