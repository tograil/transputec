using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyAggregate.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllCompanies();
    Task<IEnumerable<Company>> GetAllCompanyList(int? status, string? companyProfile);
    Task<int> CreateCompany(Company company, CancellationToken token);
}