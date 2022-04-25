using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyParameters.Repositories {
    public interface ICompanyParametersRepository {
        Task<List<CompanyParameterItem>> GetAllCompanyParameters(int companyId);
    }
}
