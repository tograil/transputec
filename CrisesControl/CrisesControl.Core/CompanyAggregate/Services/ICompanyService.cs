using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.Core.Models;

namespace CrisesControl.Core.CompanyAggregate.Services;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAllCompanies();
    Task<IEnumerable<CompanyRoot>> GetAllCompanyList(int? status, string? companyProfile);
}