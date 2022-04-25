using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories {
    public class CompanyParametersRepository : ICompanyParametersRepository {
        private readonly CrisesControlContext _context;

        public CompanyParametersRepository(CrisesControlContext context) {
            _context = context;
        }

        public async Task<List<CompanyParameterItem>> GetAllCompanyParameters(int companyId) {
            try {
                var pCompanyID = new SqlParameter("@CompanyID", companyId);

                var result = await _context.Set<CompanyParameterItem>().FromSqlRaw("exec Pro_Company_GetAllCompanyParameters {0}", pCompanyID).ToListAsync();
                return result;
            } catch (Exception ex) {
                return null;
            }
            
        }
    }
}
