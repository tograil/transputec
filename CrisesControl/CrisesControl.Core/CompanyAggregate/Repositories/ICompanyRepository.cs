using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.Core.Models;

namespace CrisesControl.Core.CompanyAggregate.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllCompanies();
    Task<IEnumerable<CompanyRoot>> GetAllCompanyList(int? status, string? companyProfile);
}