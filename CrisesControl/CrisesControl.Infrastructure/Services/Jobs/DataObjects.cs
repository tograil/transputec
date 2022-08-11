using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services.Jobs
{
    public class DataObjects
    {

        private DBCommon DBC;
        private readonly CrisesControlContext db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DataObjects(CrisesControlContext _db)
        {
            db = _db;
            DBC = new DBCommon(db, _httpContextAccessor);
            _httpContextAccessor = new HttpContextAccessor();
        }

        public async Task<CompanyDetails> GetCompanyDetails(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await db.Set<CompanyDetails>().FromSqlRaw("Pro_Admin_GetCompanyDetails @CompanyID", pCompanyId).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PreContractOffer> GetContractOffer(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await db.Set<PreContractOffer>().FromSqlRaw("exec Pro_Admin_GetCompanyDetails_ContractOffer @CompanyID", pCompanyId).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<AdminTransactionType>> GetTransactionTypes(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await db.Set<AdminTransactionType>().FromSqlRaw("exec Pro_Admin_GetCompanyDetails_TransactionTypes @CompanyID", pCompanyId).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompanyActivation> GetActivationKey(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await  db.Set<CompanyActivation>().FromSqlRaw("EXEC Pro_Admin_GetCompanyDetails_ActivationKey @CompanyID", pCompanyId).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompanyPaymentProfile> GetPaymentProfile(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await  db.Set<CompanyPaymentProfile>().FromSqlRaw("exec Pro_Admin_GetCompanyDetails_PaymentProfile @CompanyID", pCompanyId).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CompanyPackageItem>> GetCompanyPackageItem(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await  db.Set<CompanyPackageItem>().FromSqlRaw(" exec Pro_Admin_GetCompanyDetails_PackageItems @CompanyID", pCompanyId).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async  Task<IncidentPingStats> GetIncidentPingStats(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await  db.Set<IncidentPingStats>().FromSqlRaw("exec Pro_Admin_GetCompanyDetails_CompanyStats @CompanyID", pCompanyId).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<RegisteredUser> GetCompanyRegisteredUser(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await  db.Set<RegisteredUser>().FromSqlRaw("exec Pro_Admin_GetCompanyDetails_CompanyRegisteredUser @CompanyID", pCompanyId).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompanyMessageTransactionStats> GetMessageTransaction(int CompanyID)
        {
            try
            {
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var data = await  db.Set<CompanyMessageTransactionStats>().FromSqlRaw("Pro_Admin_GetCompanyDetails_MessageTransactions @CompanyID", pCompanyId).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CompaniesStats> GetCompanyGlobalReport()
        {
            try
            {
                var data = await  db.Set<CompaniesStats>().FromSqlRaw("exec Pro_Admin_GetCompanyGlobalReport").FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
