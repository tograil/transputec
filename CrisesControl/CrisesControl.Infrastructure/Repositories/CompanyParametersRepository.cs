using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.CompanyParameters.Repositories;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories {
    public class CompanyParametersRepository : ICompanyParametersRepository {
        private readonly CrisesControlContext _context;
        public CompanyParametersRepository(CrisesControlContext context)
        {
            this._context = context;
        }
        public async Task<IEnumerable<CascadingPlanReturn>> GetCascading(int PlanID, string PlanType, int CompanyId, bool GetDetails = false)
        {
            try
            {

                var pCompanyID = new SqlParameter("@CompanyId", CompanyId);
                var pPlanType = new SqlParameter("@PlanType", PlanType);
                var pPlanID = new SqlParameter("@PlanId", PlanID);
               var response = await _context.Set<CascadingPlanReturn>().FromSqlRaw("EXEC Pro_Get_Cascading_Plan @CompanyId, @PlanType, @PlanId",pCompanyID, pPlanType, pPlanID).ToListAsync();
                if (response != null)
                {
                    if (PlanID > 0)
                    {
                        var singlersp =  response.FirstOrDefault();
                        singlersp.CommsMethod = GetCascadingDetails(singlersp.PlanID, CompanyId);
                        return response;
                    }
                    else if (GetDetails)
                    {
                        response.Select(c => {
                            c.CommsMethod =GetCascadingDetails(c.PlanID, CompanyId);
                            return c;
                        });
                    }
                }

             
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return new List<CascadingPlanReturn>();
        }
        public List<CommsMethodPriority> GetCascadingDetails(int PlanID,  int CompanyId )
        {
            try
            {

                var pPlanID = new SqlParameter("@PlanId", PlanID);
                var pCompanyID = new SqlParameter("@CompanyId", CompanyId);
                var cascadingPlans =  _context.Set<CommsMethodPriority>().FromSqlRaw("EXEC Pro_Get_Cascading_Plan_Details @PlanId, @CompanyId", pCompanyID,  pPlanID).ToList();
                return cascadingPlans;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return new List<CommsMethodPriority>();
        }

        public async Task<IEnumerable<CompanyFtp>> GetCompanyFTP(int CompanyID)
        {
            try
            {
             
                var CompanyId = new SqlParameter("@CompanyID", CompanyID);
                return await _context.Set<CompanyFtp>().FromSqlRaw("EXEC Pro_Get_Company_FTP @CompanyID",  CompanyId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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
