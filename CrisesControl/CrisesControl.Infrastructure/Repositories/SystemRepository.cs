using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.System;
using CrisesControl.Core.System.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class SystemRepository : ISystemRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<SystemRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DBCommon DBC;
        public SystemRepository(CrisesControlContext context, ILogger<SystemRepository> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._context = context;
            this._logger = logger;
            this._httpContextAccessor = httpContextAccessor;
            DBC = new DBCommon(_context, _httpContextAccessor);
        }
        public async Task<string> ExportTrackingData(int TrackMeID,int UserDeviceID,DateTimeOffset StartDate, DateTimeOffset EndDate, int OutUserCompanyId)
        {
            var pTrackMeID = new SqlParameter("@TrackMeID",TrackMeID);
            var pUserDeviceID = new SqlParameter("@UserDeviceID", UserDeviceID);
            var pStartDate = new SqlParameter("@StartDate", StartDate);
            var pEndDate = new SqlParameter("@EndDate", EndDate);
  


            string FileName = "Tracking_" + TrackMeID + "_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".csv";

            string ResultFilePath = DBC.Getconfig("ImportResultPath");
            string ExportPath = ResultFilePath + OutUserCompanyId + "\\DataExport\\";
            string FilePath = ExportPath + FileName;

            if (!Directory.Exists(ExportPath))
            {
                Directory.CreateDirectory(ExportPath);
                DBC.DeleteOldFiles(ExportPath);
            }
            string headerRow = string.Empty;

            var ExportData = await  _context.Set<TrackingExport>().FromSqlRaw("exec Pro_ExportTrackingData @TrackMeID, @UserDeviceID,@StartDate,@EndDate",
                    pTrackMeID, pUserDeviceID, pStartDate, pEndDate).ToListAsync();

            using (StreamWriter SW = new StreamWriter(FilePath))
            {

                headerRow = string.Format("\"{0}\",\"{1}\",\"{2}\"", "Time Stamp", "Latitude", "Longitude");

                SW.WriteLine(headerRow);

                foreach (var row in ExportData)
                {
                    string rowdata = string.Format("\"{0}\",\"{1}\",\"{2}\"", row.CreatedOn, row.Latitude, row.Longitude);
                    if (!string.IsNullOrEmpty(rowdata))
                        SW.WriteLine(rowdata);
                }
            }
            return FileName;
        }
        public async Task<List<ModelLogReturn>> GetModelLog(DateTimeOffset StartDate, DateTimeOffset EndDate, int RecordStart, int RecordLength, string SearchString, string OrderBy, string OrderDir)
        {
            try
            {
               
                    var pStartDate = new SqlParameter("@StartDate", StartDate);
                    var pEndDate = new SqlParameter("@EndDate", EndDate);
                    var pRecordStart = new SqlParameter("@RecordStart", RecordStart);
                    var pRecordLength = new SqlParameter("@RecordLength", RecordLength);
                    var pSearchString = new SqlParameter("@SearchString", SearchString);
                    var pOrderBy = new SqlParameter("@OrderBy", OrderBy);
                    var pOrderDir = new SqlParameter("@OrderDir", OrderDir);


                    var LogData = await  _context.Set<ModelLogReturn>().FromSqlRaw("exec Pro_Get_Model_Log @StartDate, @EndDate, @RecordStart, @RecordLength, @SearchString, @OrderBy, @OrderDir",
                            pStartDate, pEndDate, pRecordStart, pRecordLength, pSearchString, pOrderBy, pOrderDir).ToListAsync();

                    return LogData;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
