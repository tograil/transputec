using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.DBCommon.Repositories;
using CrisesControl.Infrastructure.Context;
using CrisesControl.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import
{
    public class NewImport
    {
        private readonly IDBCommonRepository _DBC;
        private readonly CrisesControlContext _context;
        private readonly ImportService _importService;

        public string FilePath { get; set; }
        public string ImportFileType { get; set; }
        public string ImportType { get; set; }
        public string ColumnMappingFilePath { get; set; }
        public string ColumnMappingFileType { get; set; }
        public string ColumnDelimiter { get; set; }
        public bool FileHasHeader { get; set; }
        public bool SendInvite { get; set; }
        public bool AutoForceVerify { get; set; }
        public string SessionId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string TimeZoneId { get; set; }
        public int ImportTriggerID = 0;
        public string JobType = "FULL";

        /// <summary>
        /// New Import object, start importing of users, location and department
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="SessionId"></param>
        /// <param name="ImportType"></param>
        /// <param name="FilePath"></param>
        /// <param name="ImportFileType"></param>
        /// <param name="ColumnFilePath"></param>
        /// <param name="ColumnFileType"></param>
        /// <param name="FileHasHeader"></param>
        /// <param name="ColDelim"></param>
        public NewImport(int CompanyId, int UserId, string SessionId, string ImportType, string FilePath, string ImportFileType,
            string ColumnFilePath, string ColumnFileType, bool FileHasHeader = true, string ColDelim = ",", bool SendInvite = false,
            bool AutoForceVerify = false, string JobType = "FULL", IDBCommonRepository DBC = default, CrisesControlContext context = default, ImportService importService = default)
        {
            this.CompanyId = CompanyId;
            this.UserId = UserId;
            this.SessionId = SessionId;
            this.FilePath = FilePath;
            this.ImportFileType = ImportFileType;
            this.ImportType = ImportType;
            this.ColumnMappingFilePath = ColumnFilePath;
            this.ColumnMappingFileType = ColumnFileType;
            this.FileHasHeader = FileHasHeader;
            this.ColumnDelimiter = ColDelim;
            this.SendInvite = SendInvite;
            this.AutoForceVerify = AutoForceVerify;
            this.JobType = JobType;

            _DBC = DBC;
            _context = context;
            _importService = importService;
            //TimeZoneId = await _DBC.GetTimeZoneByCompany(CompanyId);
        }


        public void DoImport()
        {
            if (this.ImportFileType == "CSV")
            {
                ImportWithCSVFile();
            }
        }

        public void ImportWithCSVFile()
        {
            try
            {
                if (ImportType == "USER")
                {
                    DataTable importData = new DataTable();
                    StartImport(importData, ImportType);
                }
                else
                {
                    DataTable importData = ImportService.ReadCSVFile(this.FilePath, this.ColumnDelimiter, this.ColumnMappingFilePath, this.ColumnMappingFileType,
                        this.FileHasHeader);
                    StartImport(importData, ImportType);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void StartImport(DataTable importData, string importType)
        {
            if (importType == "LOCATION")
            {
                var results = ImportService.DataTableToList<DataTable>(importData);
                _importService.processLocation(results, SessionId, CompanyId, UserId, TimeZoneId);
            }
            else if (importType == "GROUP")
            {
                var results = ImportService.DataTableToList<DataTable>(importData);
                _importService.processGroup(results, SessionId, CompanyId, UserId, TimeZoneId);
            }
            else if (importType == "DEPARTMENT")
            {
                List<ImportDumpData> results = ImportService.DataTableToList<DataTable>(importData);
                //_importService.processDepartment(results, SessionId, CompanyId, UserId, TimeZoneId);
            }
            else if (importType == "USER")
            {
                //List<ImportDumpData> results = importData.DataTableToList<ImportDumpData>();
                //IH.processUsers(results, SessionId, CompanyId, UserId, TimeZoneId, SendInvite);
                BulkInsert();
                _importService.CreateImportHeader(SessionId, CompanyId, "DUMPED", UserId, Path.GetFileName(FilePath), Path.GetFileName(ColumnMappingFilePath), SendInvite,
                    ImportTriggerID, AutoForceVerify, JobType);
            }
        }

        public async Task BulkInsert()
        {
            try
            {
                DataTable importData = ImportService.ReadCSVFile(this.FilePath, this.ColumnDelimiter, this.ColumnMappingFilePath, this.ColumnMappingFileType, this.FileHasHeader);


                List<ImportDumpData> userlist = new List<ImportDumpData>();

                DateTimeOffset dtnow = await _DBC.GetDateTimeOffset(DateTime.Now, TimeZoneId);

                userlist = (from DataRow dr in importData.Rows
                            select new ImportDumpData()
                            {
                                FirstName = dr["FirstName"].ToString()!,
                                Surname = dr["Surname"].ToString()!,
                                Email = dr["Email"].ToString()!,
                                EncryptedEmail = dr["Email"].ToString()!,
                                MobileISD = dr["ISDMobile"].ToString()!,
                                Mobile = dr["Mobile"].ToString()!,
                                ISDLandline = dr["ISDLandline"].ToString()!,
                                Landline = dr["Landline"].ToString()!,
                                UserRole = dr["UserRole"].ToString()!,
                                Action = dr["Action"].ToString()!,
                                Group = dr["Group"].ToString()!,
                                GroupAction = dr["GroupAction"].ToString()!,
                                Department = dr["Department"].ToString()!,
                                DepartmentAction = dr["DepartmentAction"].ToString()!,
                                IncidentMethods = dr["IncidentMethods"].ToString()!,
                                Location = dr["Location"].ToString()!,
                                LocationAction = dr["LocationAction"].ToString()!,
                                LocationAddress = dr["LocationAddress"].ToString()!,
                                PingMethods = dr["PingMethods"].ToString()!,
                                MenuAccess = dr["MenuAccess"].ToString()!,
                                SecurityDescription = dr["SecurityDescription"].ToString()!,
                                Status = dr["Status"].ToString()!,
                                CompanyId = CompanyId,
                                SessionId = SessionId,
                                CreatedBy = UserId,
                                CreatedOn = dtnow,
                                UpdatedBy = UserId,
                                UpdatedOn = dtnow,
                                UserId = -1,
                                ActionType = "USERIMPORTCOMPLETE"
                            }).ToList();


                //IH.createTempUsers(userlist,SessionId,CompanyId,UserId,TimeZoneId);
                //using (IDataReader reader = userlist.GetDataReader())
                using (SqlConnection conn = new SqlConnection(_context.Database.GetConnectionString()))
                using (SqlBulkCopy objbulk = new SqlBulkCopy(conn))
                {

                    conn.Open();

                    objbulk.DestinationTableName = "ImportDump";
                    //Mapping Table column  
                    objbulk.ColumnMappings.Add("CompanyId", "CompanyId");
                    objbulk.ColumnMappings.Add("SessionId", "SessionId");
                    objbulk.ColumnMappings.Add("UserId", "UserId");
                    objbulk.ColumnMappings.Add("FirstName", "FirstName");
                    objbulk.ColumnMappings.Add("Surname", "Surname");
                    objbulk.ColumnMappings.Add("Email", "Email");
                    objbulk.ColumnMappings.Add("EncryptedEmail", "EncryptedEmail");
                    objbulk.ColumnMappings.Add("MobileISD", "ISD");
                    objbulk.ColumnMappings.Add("Mobile", "Phone");
                    objbulk.ColumnMappings.Add("ISDLandline", "LLISD");
                    objbulk.ColumnMappings.Add("Landline", "Landline");
                    objbulk.ColumnMappings.Add("UserRole", "UserRole");
                    objbulk.ColumnMappings.Add("Action", "Action");
                    objbulk.ColumnMappings.Add("Status", "Status");
                    objbulk.ColumnMappings.Add("Group", "Group");
                    objbulk.ColumnMappings.Add("GroupAction", "GroupAction");
                    objbulk.ColumnMappings.Add("Department", "Department");
                    objbulk.ColumnMappings.Add("DepartmentAction", "DepartmentAction");
                    objbulk.ColumnMappings.Add("Location", "Location");
                    objbulk.ColumnMappings.Add("LocationAddress", "LocationAddress");
                    objbulk.ColumnMappings.Add("LocationAction", "LocationAction");
                    objbulk.ColumnMappings.Add("MenuAccess", "Security");
                    objbulk.ColumnMappings.Add("PingMethods", "PingMethods");
                    objbulk.ColumnMappings.Add("IncidentMethods", "IncidentMethods");
                    objbulk.ColumnMappings.Add("ActionType", "ActionType");
                    objbulk.ColumnMappings.Add("CreatedBy", "CreatedBy");
                    objbulk.ColumnMappings.Add("CreatedOn", "CreatedOn");
                    objbulk.ColumnMappings.Add("UpdatedBy", "UpdatedBy");
                    objbulk.ColumnMappings.Add("UpdatedOn", "UpdatedOn");

                    //inserting bulk Records into DataBase
                    //objbulk.WriteToServer(reader);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
