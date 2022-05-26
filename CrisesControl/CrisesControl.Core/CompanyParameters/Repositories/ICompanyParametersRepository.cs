using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyParameters.Repositories
{
    public interface ICompanyParametersRepository
    {
        Task<IEnumerable<CompanyFtp>> GetCompanyFTP( int CompanyID);
        Task<IEnumerable<CascadingPlanReturn>> GetCascading(int PlanID, string PlanType, int CompanyId, bool GetDetails = false);
        List<CommsMethodPriority> GetCascadingDetails(int PlanID, int CompanyId);
        Task<List<CompanyParameterItem>> GetAllCompanyParameters(int companyId);
        Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "");
    }
}
