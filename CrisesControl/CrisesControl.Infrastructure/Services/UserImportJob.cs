using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Companies;
using CrisesControl.Core.Import;
using CrisesControl.Core.Models;
using CrisesControl.Core.Users;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Services
{
    public class UserImportJob
    {
        private DBCommon _DBC;
        private readonly CrisesControlContext _context;
        private int recordProcessError = 0;
        private int sessionProcessError = 0;
        private bool sendInvite = false;
        private bool autoForceVerify = false;
        private string timeZoneId = "GMT Standard Time";
        private ImportService _importService;
        private SendEmail _SDE;

        public UserImportJob(DBCommon DBC, ImportService importService, CrisesControlContext context, SendEmail SDE)
        {
            _DBC = DBC;
            _importService = importService;
            _context = context;
            _SDE = SDE;
        }

        public void ProcessImportJob(ImportDumpHeader header)
        {
            try
            {

                //Update import header status to IMPORTING
                _importService.CreateImportHeader(header.SessionId, header.CompanyId, "IMPORTING", header.CreatedBy, header.FileName, header.MappingFileName, (bool)header.SendInvite, 0, header.AutoForceVerify, header.JobType);

                sendInvite = (bool)header.SendInvite;
                autoForceVerify = header.AutoForceVerify;

                sessionProcessError = 0;
                bool ItemPending = true;

                _importService.CheckDataSegregation(header.CompanyId, header.CreatedBy);

                while (ItemPending)
                {
                    //Get items from import dump table for processing
                    List<ImportDumpResult> records = GetImportDump(header.SessionId, header.CompanyId, header.CreatedBy, 500);
                    //DBC.CreateLog("INFO", "Records picked up for procesing" + records.Count + " for session " + header.SessionId);

                    if (records.Count > 0)
                    {
                        foreach (ImportDumpResult rec in records)
                        {
                            recordProcessError = 0;
                            int UserID = ProcessRecord(rec);
                            //if((UserID > 0 && RecordProcessError == 0) || UserID == -1) {
                            //Move single record to dmp
                            MoveDumpToLog(rec.ImportDumpId);
                            //}
                        }
                    }
                    else
                    {
                        ItemPending = false;
                    }
                }

                //DBC.CreateLog("INFO", "Out fromt the while loop");

                if (sessionProcessError == 0)
                {
                    //Move the records to import dump log and finish he import for the session
                    MoveErrorItemsToLog(header.SessionId, header.CompanyId, header.CreatedBy);
                    _importService.CreateImportHeader(header.SessionId, header.CompanyId, "IMPORTED", header.CreatedBy, header.FileName, header.MappingFileName, (bool)header.SendInvite, JobType: header.JobType);

                    _importService.ReleaseImportQueue(header.CompanyId, header.JobType);

                    //Send the import CSV report to email.
                    SendReportToEmail(header.SessionId, header.CompanyId, header.CreatedBy);

                }
                else
                {
                    //reset the import to pick by another session.
                    _importService.CreateImportHeader(header.SessionId, header.CompanyId, "TOBEIMPORTED", header.CreatedBy, header.FileName, header.MappingFileName, (bool)header.SendInvite, JobType: header.JobType);
                }

            }
            catch (Exception ex)
            {
            }
        }



        public List<ImportDumpResult> GetImportDump(string SessionID, int CompanyID, int CreatedBy, int Limit = 1000)
        {
            try
            {
                var pSessionId = new SqlParameter("@SessionID", SessionID);
                var pCompanyId = new SqlParameter("@CompanyID", CompanyID);
                var pCreatedBy = new SqlParameter("@LoggedInUserID", CreatedBy);
                var pLimit = new SqlParameter("@LimitRecord", Limit);

                _context.Database.SetCommandTimeout(1 * 60 * 60);
                var result = _context.Set<ImportDumpResult>().FromSqlRaw("EXEC Pro_GetImportUser_FromDump @SessionID, @CompanyID, @LoggedInUserID, @LimitRecord",
                    pSessionId, pCompanyId, pCreatedBy, pLimit).ToList();
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        internal int ProcessRecord(ImportDumpResult rec)
        {
            int NewUserID = 0;
            try
            {
                _importService.ImportUsers(rec.ImportDumpId, rec.CompanyId, rec.CreatedBy, timeZoneId, sendInvite, autoForceVerify);
                NewUserID = _importService.importUserId;
                //NewUserID = CreateUser(rec);
                //if(NewUserID > 0) {
                //CreateGroup(NewUserID, rec);
                //CreateLocation(NewUserID, rec);
                //CreateSecurity(NewUserID, rec);
                //CreateUserComms(NewUserID, rec);

                //if(rec.EmailCheck.ToUpper() == "NEW" && SendInvite == true) {
                //    SendInvitation(rec.ImportDumpId);
                //}
                //}
            }
            catch (Exception ex)
            {
            }
            return NewUserID;
        }

        //internal int CreateUser(ImportDumpResult rec) {
        //    try {
        //        using(CrisesControlEntities db = new CrisesControlEntities()) {
        //            string newPwd = DBC.RandomPassword();

        //            var pImportDumpId = new SqlParameter("@ImportDumpID", rec.ImportDumpId);
        //            var pUserId = new SqlParameter("@UserID", rec.UserId);
        //            var pEmailCheck = new SqlParameter("@EmailCheck", rec.EmailCheck);
        //            var pAction = new SqlParameter("@Action", rec.Action);
        //            var pCompanyId = new SqlParameter("@CompanyID", rec.CompanyId);

        //            var pFirstName = new SqlParameter("@FirstName", System.Data.SqlDbType.NVarChar, 70);
        //            pFirstName.Value = rec.FirstName ?? "";

        //            var pSurname = new SqlParameter("@LastName", System.Data.SqlDbType.NVarChar, 70);
        //            pSurname.Value = rec.Surname ?? "";

        //            var pPhone = new SqlParameter("@MobileNo", System.Data.SqlDbType.NVarChar, 20);
        //            pPhone.Value = rec.Phone;

        //            var pEmail = new SqlParameter("@PrimaryEmail", System.Data.SqlDbType.NVarChar, 150);
        //            pEmail.Value = rec.Email ?? "";

        //            var pPassword = new SqlParameter("@Password", System.Data.SqlDbType.NVarChar, 50);
        //            pPassword.Value = newPwd;

        //            var pISD = new SqlParameter("@ISDCode", System.Data.SqlDbType.NVarChar, 10);
        //            pISD.Value = rec.ISD ?? "";

        //            var pUserRole = new SqlParameter("@UserRole", rec.UserRole ?? "");
        //            var pLLISD = new SqlParameter("@LLISDCode", System.Data.SqlDbType.NVarChar, 10);
        //            pLLISD.Value = rec.LLISD ?? "";

        //            var pLandline = new SqlParameter("@Landline", System.Data.SqlDbType.NVarChar, 20);
        //            pLandline.Value = rec.Landline ?? "";

        //            var pCreatedBy = new SqlParameter("@LogggedInUser", rec.CreatedBy);

        //            db.Database.CommandTimeout = 1 * 60 * 60;
        //            var UserOnlyrec = db.Database.SqlQuery<JsonResult>("UpsertUser_FromDump @ImportDumpID, @UserID, @EmailCheck, @Action, @CompanyID, @FirstName, @LastName, @MobileNo, @PrimaryEmail, @Password, @ISDCode, @UserRole, @LLISDCode, @Landline, @LogggedInUser",
        //                pImportDumpId, pUserId, pEmailCheck, pAction, pCompanyId, pFirstName, pSurname, pPhone, pEmail, pPassword, pISD, pUserRole, pLLISD, pLandline, pCreatedBy).FirstOrDefault();

        //            if(UserOnlyrec != null) {
        //                CCUser.UserHelper UH = new CCUser.UserHelper();

        //                var userUpdate = (from U in db.Users
        //                                  where U.UserId == UserOnlyrec.UserID select U).FirstOrDefault();
        //                if(userUpdate != null) {
        //                    UH.CreateUserSearch(userUpdate.UserId, userUpdate.FirstName, userUpdate.LastName, userUpdate.ISDCode, userUpdate.MobileNo, userUpdate.PrimaryEmail, userUpdate.CompanyId);
        //                    UH.CreateSMSTriggerRight(userUpdate.CompanyId, userUpdate.UserId, userUpdate.UserRole, false, userUpdate.ISDCode, userUpdate.MobileNo, true);
        //                }

        //                return UserOnlyrec.UserID;
        //            }
        //        }
        //    } catch(Exception ex) {
        //        RecordProcessError++;
        //        DBC.catchException(ex);
        //    }
        //    return 0;
        //}

        //internal void CreateGroup(int NewUserID, ImportDumpResult rec) {
        //    try {
        //        using(CrisesControlEntities db = new CrisesControlEntities()) {
        //            //EXEC[dbo].[UpsertUser_Group_FromDump] @ImportDumpID, @NewUserID, @CompanyID, @Group, @GroupStatus, 
        //            //                @GroupAction, @GroupCheck, @GroupId, @LogggedInUser

        //            var pImportDumpId = new SqlParameter("@ImportDumpID", rec.ImportDumpId);
        //            var pUserId = new SqlParameter("@UserID", NewUserID);
        //            var pCompanyId = new SqlParameter("@CompanyID", rec.CompanyId);
        //            var pGroup = new SqlParameter("@Group", rec.Group);
        //            var pGroupStatus = new SqlParameter("@GroupStatus", rec.GroupStatus);
        //            var pGroupAction = new SqlParameter("@GroupAction", rec.GroupAction);
        //            var pGroupCheck = new SqlParameter("@GroupCheck", rec.GroupCheck);
        //            var pGroupId = new SqlParameter("@GroupId", rec.GroupId);
        //            var pCreatedBy = new SqlParameter("@LogggedInUser", rec.CreatedBy);

        //            db.Database.CommandTimeout = 1 * 60 * 60;
        //            db.Database.ExecuteSqlCommand("UpsertUser_Group_FromDump @ImportDumpID, @UserID, @CompanyID, @Group, @GroupStatus, @GroupAction, @GroupCheck, @GroupId, @LogggedInUser",
        //                pImportDumpId, pUserId, pCompanyId, pGroup, pGroupStatus, pGroupAction, pGroupCheck, pGroupId, pCreatedBy);
        //        }
        //    } catch(Exception ex) {
        //        RecordProcessError++;
        //        DBC.catchException(ex);
        //    }


        //}

        //internal void CreateLocation(int NewUserID, ImportDumpResult rec) {
        //    try {
        //        using(CrisesControlEntities db = new CrisesControlEntities()) {
        //            //EXEC[dbo].[UpsertUser_Location_FromDump] @ImportDumpID, @NewUserID, @CompanyID, @Location, @LocationAddress, @LocationStatus, @LocationAction, 
        //            //@LocationCheck, @LocationId, @LocLat, @LocLng, @LogggedInUser

        //            var pImportDumpId = new SqlParameter("@ImportDumpID", rec.ImportDumpId);
        //            var pUserId = new SqlParameter("@UserID", NewUserID);
        //            var pCompanyId = new SqlParameter("@CompanyID", rec.CompanyId);
        //            var pLocation = new SqlParameter("@Location", rec.Location);
        //            var pLocationAddress = new SqlParameter("@LocationAddress", rec.LocationAddress);
        //            var pLocationStatus = new SqlParameter("@LocationStatus", rec.LocationStatus);
        //            var pLocationAction = new SqlParameter("@LocationAction", rec.LocationAction);
        //            var pLocationCheck = new SqlParameter("@LocationCheck", rec.LocationCheck);
        //            var pLocationId = new SqlParameter("@LocationId", rec.LocationId);
        //            var pLocLat = new SqlParameter("@LocLat", rec.LocLat);
        //            var pLocLng = new SqlParameter("@LocLng", rec.LocLng);
        //            var pCreatedBy = new SqlParameter("@LogggedInUser", rec.CreatedBy);

        //            db.Database.CommandTimeout = 1 * 60 * 60;
        //            db.Database.ExecuteSqlCommand("UpsertUser_Location_FromDump @ImportDumpID, @UserID, @CompanyID, @Location, @LocationAddress, @LocationStatus, @LocationAction, @LocationCheck, @LocationId, @LocLat, @LocLng, @LogggedInUser",
        //                pImportDumpId, pUserId, pCompanyId, pLocation, pLocationAddress, pLocationStatus, pLocationAction, pLocationCheck, pLocationId, pLocLat, pLocLng, pCreatedBy);

        //        }
        //    } catch(Exception ex) {
        //        RecordProcessError++;
        //        DBC.catchException(ex);
        //    }
        //}

        //internal void CreateSecurity(int NewUserID, ImportDumpResult rec) {
        //    try {
        //        using(CrisesControlEntities db = new CrisesControlEntities()) {
        //            //EXEC[dbo].[UpsertUser_Security_FromDump] @ImportDumpID, @NewUserID, @CompanyID, @SecurityCheck, @SecurityGroupId, @LogggedInUser

        //            var pImportDumpId = new SqlParameter("@ImportDumpID", rec.ImportDumpId);
        //            var pUserId = new SqlParameter("@UserID", NewUserID);
        //            var pCompanyId = new SqlParameter("@CompanyID", rec.CompanyId);
        //            var pSecurityCheck = new SqlParameter("@SecurityCheck", rec.SecurityCheck);
        //            var pSecurityGroupId = new SqlParameter("@SecurityGroupId", rec.SecurityGroupId);
        //            var pCreatedBy = new SqlParameter("@LogggedInUser", rec.CreatedBy);

        //            db.Database.CommandTimeout = 1 * 60 * 60;
        //            db.Database.ExecuteSqlCommand("UpsertUser_Security_FromDump @ImportDumpID, @UserID, @CompanyID, @SecurityCheck, @SecurityGroupId, @LogggedInUser",
        //                pImportDumpId, pUserId, pCompanyId, pSecurityCheck, pSecurityGroupId, pCreatedBy);
        //        }
        //    } catch(Exception ex) {
        //        RecordProcessError++;
        //        DBC.catchException(ex);
        //    }
        //}

        //internal void CreateUserComms(int NewUserID, ImportDumpResult rec) {
        //    try {
        //        using(CrisesControlEntities db = new CrisesControlEntities()) {
        //            //EXEC[dbo].[UpsertUser_ComMethods_FromDump] @ImportDumpID, @NewUserID, @CompanyID, @PingMethods, @IncidentMethods, @LogggedInUser

        //            var pImportDumpId = new SqlParameter("@ImportDumpID", rec.ImportDumpId);
        //            var pUserId = new SqlParameter("@UserID", NewUserID);
        //            var pCompanyId = new SqlParameter("@CompanyID", rec.CompanyId);
        //            var pPingMethods = new SqlParameter("@PingMethods", rec.PingMethods);
        //            var pIncidentMethods = new SqlParameter("@IncidentMethods", rec.IncidentMethods);
        //            var pCreatedBy = new SqlParameter("@LogggedInUser", rec.CreatedBy);

        //            db.Database.CommandTimeout = 1 * 60 * 60;
        //            db.Database.ExecuteSqlCommand("UpsertUser_ComMethods_FromDump @ImportDumpID, @UserID, @CompanyID, @PingMethods, @IncidentMethods, @LogggedInUser",
        //                pImportDumpId, pUserId, pCompanyId, pPingMethods, pIncidentMethods, pCreatedBy);
        //        }
        //    } catch(Exception ex) {
        //        RecordProcessError++;
        //        DBC.catchException(ex);
        //    }
        //}

        internal void MoveDumpToLog(int ImportDumpId)
        {
            try
            {
                var pImportDumpId = new SqlParameter("@ImportDumpId", ImportDumpId);

                _context.Database.ExecuteSqlRaw("EXEC Pro_ImportUser_MoveDumpRecordToLog @ImportDumpId", pImportDumpId);
            }
            catch (Exception ex)
            {
            }
        }

        public void MoveErrorItemsToLog(string SessionID, int CompanyID, int CreatedBy)
        {
            try
            {
                var pSessionID = new SqlParameter("@SessionID", SessionID);
                var pCompanyID = new SqlParameter("@CompanyID", CompanyID);
                var pCreatedBy = new SqlParameter("@LoggedInUserID", CreatedBy);

                _context.Database.SetCommandTimeout(1 * 60 * 60);
                var Com = _context.Database.ExecuteSqlRaw("EXEC Pro_ImportUser_MoveResidualDumpRecordsToLog @SessionID, @CompanyID, @LoggedInUserID",
                    pSessionID, pCompanyID, pCreatedBy);
            }
            catch (Exception ex)
            {
            }
        }

        internal void SendInvitation(int ImportDumpId)
        {
            try
            {
                 var pImportDumpId = new SqlParameter("@ImportDumpId", ImportDumpId);

                var result = _context.Set<EmailNotificationToUser>().FromSqlRaw("EXEC Pro_ImportUser_NotificationDetails @ImportDumpId", pImportDumpId).FirstOrDefault();

                if (result != null)
                {
                    if (!string.IsNullOrEmpty(result.ToEMail))
                    {
                        string[] ToEmail = { result.ToEMail };
                        bool issent = _SDE.Email(ToEmail, result.MessageBody, result.FromEMail, result.HostName, result.Subject);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void SendReportToEmail(string SessionId, int CompanyId, int CreatedBy)
        {
            try
            {

                    ImportSummary Summary = GetImportSummary(SessionId);

                    string Subject = string.Empty;
                    string ParamsInfo = "";

                    string message = Convert.ToString(_DBC.ReadHtmlFile("IMPORT_PROCESS_EMAIL", "DB", CompanyId, out Subject));


                    string ReportEmail = (from U in _context.Set<User>() where U.UserId == CreatedBy && U.Status == 1 select U.PrimaryEmail).FirstOrDefault();

                    var impheader = (from IH in _context.Set<ImportDumpHeader>() where IH.SessionId == SessionId select IH).FirstOrDefault();
                    if (impheader != null)
                    {
                        if (impheader.ImportTriggerId > 0)
                        {
                            ReportEmail = (from TR in _context.Set<ExTrigger>() where TR.ExTriggerId == impheader.ImportTriggerId select TR.FailureReportEmail).FirstOrDefault();
                        }
                        var import_type = (from IM in _context.Set<ImportDumpLog>() where IM.SessionId == SessionId select IM).FirstOrDefault();
                        if (import_type != null)
                        {
                            if (import_type.ActionType == "USERIMPORTCOMPLETE")
                            {
                                string override_phone = _DBC.GetCompanyParameter("OVERRIDE_PHONE_NUMBER", CompanyId);
                                string override_dummy = _DBC.GetCompanyParameter("OVERRIDE_DUMMY_NUMBER_ONLY", CompanyId);

                                if (override_phone == "false")
                                {
                                    ParamsInfo += "<br />Override phone number is turned off";
                                }
                                if (override_dummy == "true")
                                {
                                    ParamsInfo += "<br />Override dummy phone number is turned on";
                                }

                                if (!string.IsNullOrWhiteSpace(ParamsInfo))
                                {
                                    ParamsInfo = "<strong>Import Parameters</strong>" + ParamsInfo;
                                }
                            }
                        }
                    }

                    string[] sendEmailTo = ReportEmail.Split(new char[] { ';', ',' });

                    if (sendEmailTo.Length > 0 && !string.IsNullOrEmpty(message))
                    {

                        string FromEmail = _DBC.LookupWithKey("EMAILFROM");
                        string HostName = _DBC.LookupWithKey("SMTPHOST");
                        string CCLogo = _DBC.LookupWithKey("CCLOGO");
                        string Portal = _DBC.LookupWithKey("PORTAL");
                        string ReportLink = Portal + "dataimport/downloadresult/" + SessionId;

                        var CompanyInfo = (from C in _context.Set<Company>() where C.CompanyId == CompanyId select C).FirstOrDefault();

                        string CompanyLogo = Portal + "/uploads/" + CompanyInfo.CompanyId + "/companylogos/" + CompanyInfo.CompanyLogoPath;
                        if (string.IsNullOrEmpty(CompanyInfo.CompanyLogoPath))
                        {
                            CompanyLogo = CCLogo;
                        }

                        string messagebody = message;

                        messagebody = messagebody.Replace("{RECIPIENT_NAME}", _DBC.UserName(Summary.UserName));
                        messagebody = messagebody.Replace("{COMPANY_NAME}", Summary.CompanyName);
                        messagebody = messagebody.Replace("{CUSTOMER_ID}", Summary.CustomerId);
                        messagebody = messagebody.Replace("{COMPANY_LOGO}", CompanyLogo);
                        messagebody = messagebody.Replace("{TOTAL_IMPORTED}", Summary.TotalImport.ToString());
                        messagebody = messagebody.Replace("{TOTAL_DELETED}", Summary.TotalDelete.ToString());
                        messagebody = messagebody.Replace("{TOTAL_UPDATE}", Summary.TotalUpdate.ToString());
                        messagebody = messagebody.Replace("{TOTAL_ERROR}", Summary.TotalSkip.ToString());
                        messagebody = messagebody.Replace("{REPORT_LINK}", ReportLink);
                        messagebody = messagebody.Replace("{IMPORT_PARAMS}", ParamsInfo);

                        _SDE.Email(sendEmailTo, messagebody, FromEmail, HostName, Subject, null);

                    }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ImportSummary GetImportSummary(string SessionID)
        {
            try
            {
                var pSessionId = new SqlParameter("@SessionId", SessionID);

                _context.Database.SetCommandTimeout(1 * 60 * 60);
                var result = _context.Set<ImportSummary>().FromSqlRaw("EXEC Pro_Import_Job_Summary @SessionId", pSessionId).FirstOrDefault();
                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new ImportSummary();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
