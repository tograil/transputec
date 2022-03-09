using CrisesControl.Api.Application.ViewModels.Company;

namespace CrisesControl.Api.Application.Query;

public interface ICompanyQuery
{
    Task<IEnumerable<CompanyInfo>> GetCompanyList(int? status, string? companyProfile);
}