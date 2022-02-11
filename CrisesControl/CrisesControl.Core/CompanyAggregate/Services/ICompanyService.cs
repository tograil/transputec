using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyModel = CrisesControl.Core.Models.Company;

namespace CrisesControl.Core.CompanyAggregate.Services;

public interface ICompanyService
{
    Task<IEnumerable<CompanyModel>> GetAllCompanies();
}