using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Companies.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllCompanies();
    Task<IEnumerable<Company>> GetAllCompanyList(int? status, string? companyProfile);
    Task<string> GetCompanyParameter(string key, int companyId, string @default = "", string customerId = "");
    Task<int> CreateCompany(Company company, CancellationToken token);

    Task<string> GetTimeZone(int companyId);
}