using CrisesControl.Api.Application.Commands.Companies.GetCompany;
using CrisesControl.Api.Application.ViewModels.Company;

namespace CrisesControl.Api.Application.Query;

public interface ICompanyQuery
{
    public Task<IEnumerable<CompanyInfo>> GetCompanyList(int? status, string? companyProfile);
    public Task<GetCompanyResponse> GetCompany(GetCompanyRequest request, CancellationToken cancellationToken);
}