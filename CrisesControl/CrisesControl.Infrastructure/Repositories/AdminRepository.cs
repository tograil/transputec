
using CrisesControl.Core.Administrator;
using CrisesControl.Core.Administrator.Repositories;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Models;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class AdminRepository: IAdminRepository
    {
        private readonly CrisesControlContext _context;
        private readonly ILogger<AdminRepository> _logger;
        public AdminRepository(CrisesControlContext context, ILogger<AdminRepository> logger)
        {
            this._context=context;
            this._logger=logger;
        }
        public async Task<List<LibIncident>> GetAllLibIncident()
        {
            try
            {
                var allLibIncidents = await _context.Set<LibIncident>("exec Pro_Admin_GetAllLibIncidents").ToListAsync();

                if (allLibIncidents != null)
                {
                    return allLibIncidents;
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new List<LibIncident>();
        }
        public async Task<DataTable> GetReportData(int ReportID, List<ReportParam> sqlParams, string rFilePath,  string rFileName)
        {
            DataTable dt = new DataTable();
            rFilePath = string.Empty;
            rFileName = string.Empty;
            try
            {

                string ResultFilePath = Getconfig("ImportResultPath");
                string ExportPath = ResultFilePath + "DataExport\\";

                connectUNCPath();

                var reports = await  GetReportList(ReportID);

                if (reports != null)
                {

                    List<AdminReport> objdata = JsonConvert.DeserializeObject<List<AdminReport>>(reports.Result);
                    var ReportSP = objdata.FirstOrDefault().ReportSource;
                    var SourceType = objdata.FirstOrDefault().SourceType;
                    string FileName = objdata.FirstOrDefault().DownloadFileName + ".csv";

                    if (objdata.FirstOrDefault().DownloadFileName.ToUpper() == "RANDOM")
                        FileName = Guid.NewGuid().ToString() + ".csv";

                    if (SourceType == "VIEW" || SourceType == "TABLE")
                    {
                        ReportSP = "SELECT * FROM " + ReportSP;
                    }

                    string FilePath = ExportPath + FileName;
                    rFilePath = FilePath;
                    rFileName = FileName;

                    if (!Directory.Exists(ExportPath))
                    {
                        Directory.CreateDirectory(ExportPath);
                        DeleteOldFiles(ExportPath);
                    }

                    if (File.Exists(FilePath))
                    {
                        File.Delete(FilePath);
                    }

                    string constr = System.Configuration.ConfigurationManager.ConnectionStrings.ToString();
                    using (SqlConnection con = new SqlConnection(constr))
                    {

                        if (SourceType == "SP")
                        {
                            string delim = "";
                            ReportSP += " ";
                            foreach (ReportParam param in sqlParams)
                            {
                                ReportSP += delim + "@" + param.ParamName;
                                delim = ", ";
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(ReportSP))
                        {

                            foreach (ReportParam param in sqlParams)
                            {
                                cmd.Parameters.AddWithValue("@" + param.ParamName, param.ParamValue);
                            }

                            using (SqlDataAdapter sda = new SqlDataAdapter())
                            {
                                cmd.Connection = con;
                                con.Open();
                                sda.SelectCommand = cmd;
                                sda.Fill(dt);
                            }
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
                return dt;
            }

        }

      
        private void DeleteOldFiles(string dirName)
        {
            string[] files = Directory.GetFiles(@dirName);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddDays(-1))
                    fi.Delete();
            }
        }
        private string Getconfig(string key, string DefaultVal = "")
        {
            try
            {
                string value = System.Configuration.ConfigurationManager.AppSettings[key];
                if (value != null)
                {
                    return value;
                }
                else
                {
                    return DefaultVal;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                return DefaultVal;
            }
        }

        public bool connectUNCPath(string UNCPath = "", string strUncUsername = "", string strUncPassword = "", string UseUNC = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(UNCPath))
                    UNCPath = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UNCPath"]);
                if (!string.IsNullOrEmpty(UseUNC))
                    UseUNC = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UseUNC"]);
                if (!string.IsNullOrEmpty(strUncUsername))
                    strUncUsername = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncUserName"]);
                if (!string.IsNullOrEmpty(strUncPassword))
                    strUncPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UncPassword"]);

                if (UseUNC == "true")
                {
                    UNCAccessWithCredentials.disconnectRemote(@UNCPath);
                    if (string.IsNullOrEmpty(UNCAccessWithCredentials.connectToRemote(@UNCPath, strUncUsername, strUncPassword)))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
           
            }
           
        }
       
        public async Task<string> ToCSVHighPerformance(DataTable dataTable, bool includeHeaderAsFirstRow = true, string separator = ",")
        {
            //DataTable dataTable = new DataTable();
            StringBuilder csvRows = new StringBuilder();
            string row = "";
            int columns;
            try
            {
                //dataTable.Load(dataReader);
                columns = dataTable.Columns.Count;
                //Create Header
                if (includeHeaderAsFirstRow)
                {
                    for (int index = 0; index < columns; index++)
                    {
                        row += (dataTable.Columns[index]);
                        if (index < columns - 1)
                            row += (separator);
                    }
                    row += (Environment.NewLine);
                }
                csvRows.Append(row);

                //Create Rows
                for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
                {
                    row = "";
                    //Row
                    for (int index = 0; index < columns; index++)
                    {
                        string value = dataTable.Rows[rowIndex][index].ToString();

                        //If type of field is string
                        if (dataTable.Rows[rowIndex][index] is string)
                        {
                            //If double quotes are used in value, ensure each are replaced by double quotes.
                            if (value.IndexOf("\"") >= 0)
                                value = value.Replace("\"", "\"\"");

                            //If separtor are is in value, ensure it is put in double quotes.
                            if (value.IndexOf(separator) >= 0)
                                value = "\"" + value + "\"";

                            //If string contain new line character
                            while (value.Contains("\r"))
                            {
                                value = value.Replace("\r", "");
                            }
                            while (value.Contains("\n"))
                            {
                                value = value.Replace("\n", "");
                            }
                        }
                        row += value;
                        if (index < columns - 1)
                            row += separator;
                    }
                    dataTable.Rows[rowIndex][columns - 1].ToString().ToString().Replace(separator, " ");
                    row += Environment.NewLine;
                    csvRows.Append(row);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            dataTable.Dispose();
            return csvRows.ToString();
        }
        public async Task<AdminResult> GetReportList(int ReportID)
        {
            try
            {
                var pReportID = new SqlParameter("@ReportID", ReportID);
                var result = await _context.Set<AdminResult>().FromSqlRaw("exec Report_Admin_Get @ReportID", pReportID).FirstOrDefaultAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
                return null;
            }
        }
        public async Task<LibIncidentType> GetLibIncidentTypeByName(string Name)
        {
            var LibIncidentTypeExist = await _context.Set<LibIncidentType>().Where(LIT => LIT.Name == Name).FirstOrDefaultAsync();
            return LibIncidentTypeExist;
        }
        public async Task<LibIncident> GetLibIncidentByName(string Name)
        {
            var LibIncidentTypeExist = await _context.Set<LibIncident>().Where(LIT=> LIT.Name == Name).FirstOrDefaultAsync();
            return LibIncidentTypeExist;
        }
        public async Task<int> UpdateLibIncidentType(LibIncidentType libIncidentType)
        {
            try
            {
                _context.Update(libIncidentType);
                await _context.SaveChangesAsync();
                return libIncidentType.LibIncidentTypeId;
                              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddLibIncidentType(LibIncidentType libIncidentType)
        {
            try
            {
                await _context.AddAsync(libIncidentType);
                await _context.SaveChangesAsync();
                return libIncidentType.LibIncidentTypeId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> UpdateLibIncident(LibIncident libIncident)
          {
            try
            {
                _context.Update(libIncident);
               await _context.SaveChangesAsync();
                
                return libIncident.LibIncidentId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> AddLibIncident(LibIncident libIncident)
        {
            try
            {
                await _context.AddAsync(libIncident);
                await _context.SaveChangesAsync();
                return libIncident.LibIncidentId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> DeleteLibIncidentType(LibIncidentType libIncidentType )
        {
            try
            {
                
              _context.Remove(libIncidentType);
              await _context.SaveChangesAsync();               
              return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }
        public async Task<AdminLibIncident> GetLibIncident(int LibIncidentId)
        {

            try
            {
                var pLibIncidentId = new SqlParameter("@LibIncidentID", LibIncidentId);
                var libIncident = await _context.Set<AdminLibIncident>().FromSqlRaw("exec Pro_Admin_GetLibIncident @LibIncidentID", pLibIncidentId).FirstOrDefaultAsync();

                return libIncident;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LibIncident> GetLibIncidentById(int LibIncidentId)
        {
            var LibIncident = await _context.Set<LibIncident>().Where(LIT => LIT.LibIncidentId == LibIncidentId).FirstOrDefaultAsync();
            return LibIncident;
        }
        public async Task<bool> DeleteLibIncident(LibIncident libIncident)
        {
            try
            {
               
               _context.Remove(libIncident);
               await _context.SaveChangesAsync();
                    return true;
                
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<LibIncidentType>> GetAllLibIncidentType()
        {
            try
            {
                //Use: [dbo].[Pro_Admin_GetAllLibraryIncidentType]
                var allLibIncidentType = await _context.Set<LibIncidentType>().FromSqlRaw("exec Pro_Admin_GetAllLibraryIncidentType").ToListAsync();
                return allLibIncidentType;
        
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<LibIncidentType> GetLibIncidentType(int LibIncidentTypeId)
        {
            try
            {
                //Use: EXEC [dbo].[Pro_Admin_GetLibraryIncidentType] @LibIncidentTypeId
                var pLibIncidentTypeID = new SqlParameter("@LibIncidentTypeId", LibIncidentTypeId);
                var libIncidentType = await _context.Set<LibIncidentType>().FromSqlRaw("exec Pro_Admin_GetLibraryIncidentType @LibIncidentTypeId", pLibIncidentTypeID).FirstOrDefaultAsync();

                return libIncidentType;
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LibIncidentType> GetLibIncidentTypeById(int LibIncidentTypeId)
        {
            var LibIncident = await _context.Set<LibIncidentType>().Where(LIT => LIT.LibIncidentTypeId == LibIncidentTypeId).FirstOrDefaultAsync();
            return LibIncident;
        }
        public async Task<List<CompanyPackageFeatureList>> GetCompanyPackageFeatures(int OutUserCompanyId)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", OutUserCompanyId);
                var transactions = await _context.Set<CompanyPackageFeatureList>().FromSqlRaw(" exec Pro_Company_PackageFeature @CompanyID", pCompanyID).ToListAsync();

                    return transactions;
               
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<CompanyPackageFeatureList>> GetCompanyModules(int OutUserCompanyId)
        {
            try
            {
                var pCompanyID = new SqlParameter("@CompanyID", OutUserCompanyId);
                var transactions = await _context.Set<CompanyPackageFeatureList>().FromSqlRaw("exec Pro_Company_Module @CompanyID", pCompanyID).ToListAsync();

               
                    return transactions;
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
