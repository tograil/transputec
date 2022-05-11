using CrisesControl.Core.Compatibility;
using CrisesControl.Core.Reports;
using CrisesControl.Core.Reports.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories {
    public class ReportRepository : IReportsRepository {
        private readonly CrisesControlContext _context;
        private readonly ILogger<ReportRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private int UserID;
        private int CompanyID;
        private string UniqueKey;

        public ReportRepository(CrisesControlContext context, IHttpContextAccessor httpContextAccessor, ILogger<ReportRepository> logger) {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            this._logger = logger;
        }
        public async Task<List<SOSItem>> GetSOSItems() {
            try {
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));

                var pUserId = new SqlParameter("@UserID", UserID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);

                var result = await _context.Set<SOSItem>().FromSqlRaw("exec Pro_Get_SOS_Alerts {0},{1}", pCompanyID, pUserId).ToListAsync();

                return result;

            } catch (Exception ex) {
                return null;
            }
        }

        public async Task<List<IncidentPingStatsCount>> GetIncidentPingStats(int CompanyID, int NoOfMonth) {
            try {

                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pNoOfMonth = new SqlParameter("@NoOfMonth", NoOfMonth);

                var result = await _context.Set<IncidentPingStatsCount>().FromSqlRaw("exec Pro_Report_Dashboard_Incident_Ping_Stats_ByMonth {0},{1}", pCompanyID, pNoOfMonth).ToListAsync();
                return result;


            } catch (Exception ex) {
            }
            return new List<IncidentPingStatsCount>();
        }

        public async Task<List<DataTablePaging>> GetIndidentMessageNoAck(int draw, int IncidentActivationId, int RecordStart, int RecordLength, string? SearchString, string? UniqueKey)
        {
            try
            {
                const string OrderBy = "UserId";
                const string OrderDir = "asc";
                UserID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("sub"));
                CompanyID = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue("company_id"));
                var pIncidentActivationId = new SqlParameter("@ActiveIncidentID", IncidentActivationId);
                var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                var pSearchString = new SqlParameter("@SearchString", SearchString);
                var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                var pOrderDir = new SqlParameter("@OrderDir", OrderDir);
                 var pUserID = new SqlParameter("@UserID", UserID);
                 var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pUniqueKey = new SqlParameter("@UniqueKey", UniqueKey);

                var result = await _context.Set<MessageAcknowledgements>().FromSqlRaw("exec Pro_Get_No_Ack_Users @ActiveIncidentID, @UserID, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir, @UniqueKey",
                   pIncidentActivationId, pUserID, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir, pUniqueKey).ToListAsync();

                 int totalRecord = 0;
                totalRecord = result.Count;

               List<DataTablePaging> rtn = new List<DataTablePaging>(await _context.Set<DataTablePaging>().Where(x=>x.draw== draw).Select(n=>
                new DataTablePaging() { 
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = result.Count,
                data = result
                }    ).ToListAsync());
                return rtn;
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred while seeding the database  {Error} {StackTrace} {InnerException} {Source}",
                                          ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
               
            }
            return new List<DataTablePaging>();


        }
    }
}
