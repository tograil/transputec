using CrisesControl.Api.Application.Helpers;
using CrisesControl.Core.Common;
using CrisesControl.Core.CustomEventLog;
using CrisesControl.Core.CustomEventLog.Repositories;
using CrisesControl.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Infrastructure.Repositories
{
    public class CustomEventLogRepository : ICustomEventLogRepository
    {
        private readonly CrisesControlContext _context;
        private readonly IDBCommonRepository _DBC;
        public CustomEventLogRepository(CrisesControlContext context, IDBCommonRepository DBC)
        {
            _context = context;
            _DBC = DBC;
        }
        public Task<ExportLogResponse> ExportEventLog(EventLogModel eventLogModel)
        {
            throw new NotImplementedException();
        }

        public async Task<EventLogListing> GetEventLog(int eventLogId, int eventLogHeaderId, int companyId, int userId)
        {
            try
            {
                var pEventLogHeaderID = new SqlParameter("@eventLogHeaderId", eventLogHeaderId);
                var pEventLogID = new SqlParameter("@eventLogId", eventLogId);
                var pCompanyID = new SqlParameter("@companyId", companyId);
                var pUserID = new SqlParameter("@userId", userId);

                var result = await _context.Set<EventLogListing>().FromSqlRaw("EXEC Custom_Get_EventLog @eventLogHeaderId, @eventLogId, @companyId, @userId",
                    pEventLogHeaderID, pEventLogID, pCompanyID, pUserID).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<EventLogHeader> GetEventLogHeader(int activeIncidentId, int eventLogHeaderId, int companyId, int userId)
        {
            try
            {
                var pActiveIncidentID = new SqlParameter("@activeIncidentId", activeIncidentId);
                var pEventLogHeaderID = new SqlParameter("@eventLogHeaderId", eventLogHeaderId);
                var pCompanyID = new SqlParameter("@companyId", companyId);
                var pUserID = new SqlParameter("@userId", userId);

                var result = await _context.Set<EventLogHeader>().FromSqlRaw("EXEC Custom_Get_EventLog_Header @eventLogHeaderId, @activeIncidentId, @companyId, @userId",
                    pEventLogHeaderID, pActiveIncidentID, pCompanyID, pUserID).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<List<EventLogListing>> GetLogs(int activeIncidentId, int eventLogHeaderId, int companyId, int userId)
        {
            try
            {
                var pEventLogHeaderID = new SqlParameter("@eventLogHeaderId", eventLogHeaderId);
                var pActiveIncidentID = new SqlParameter("@activeIncidentId", activeIncidentId);
                var pCompanyID = new SqlParameter("@companyId", companyId);
                var pUserID = new SqlParameter("@userId", userId);

                var result = _context.Set<EventLogListing>().FromSqlRaw("EXEC Custom_Get_EventLog_List @eventLogHeaderId, @activeIncidentId, @companyId, @userId",
                    pEventLogHeaderID, pActiveIncidentID, pCompanyID, pUserID).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<EventMessageLog>> GetMessageLog(int eventLogId, int companyId, int userId)
        {
            try
            {
                var pEventLogID = new SqlParameter("@eventLogId", eventLogId);
                var pCompanyID = new SqlParameter("@companyId", companyId);
                var pUserID = new SqlParameter("@userId", userId);

                var result = await _context.Set<EventMessageLog>().FromSqlRaw("EXEC Custom_Get_Event_Message_Log @eventLogId, @companyId, @userId",
                    pEventLogID, pCompanyID, pUserID).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> SaveEventLog(EventLogEntry eventLogEntry, int userId, string timeZoneId)
        {
            try
            {
                DateTimeOffset dtNow = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                var pEventLogID = new SqlParameter("@EventLogID", eventLogEntry.EventLogID);
                var pEventLogHeaderID = new SqlParameter("@EventLogHeaderID", eventLogEntry.EventLogHeaderId);
                var pLogEntryDateTime = new SqlParameter("@LogEntryDateTime", eventLogEntry.LogEntryDateTime);
                var pIncidentDetails = new SqlParameter("@IncidentDetails", eventLogEntry.IncidentDetails);
                var pSourceOfInformation = new SqlParameter("@SourceOfInformation", eventLogEntry.SourceOfInformation);
                var pIsConfirmed = new SqlParameter("@IsConfirmed", eventLogEntry.IsConfirmed);
                var pCMTAction = new SqlParameter("@CMTAction", eventLogEntry.CMTAction);
                var pActionPriority = new SqlParameter("@ActionPriority", eventLogEntry.ActionPriority);
                var pActionUser = new SqlParameter("@ActionUser", eventLogEntry.ActionUser);
                var pActionGroup = new SqlParameter("@ActionGroup", eventLogEntry.ActionGroup);
                var pActionDueBy = new SqlParameter("@ActionDueBy", eventLogEntry.ActionDueBy);
                var pStatusOfAction = new SqlParameter("@StatusOfAction", eventLogEntry.StatusOfAction);
                var pActionDetail = new SqlParameter("@ActionDetail", eventLogEntry.ActionDetail);
                var pComments = new SqlParameter("@Comments", eventLogEntry.Comments);
                var pActionedDate = new SqlParameter("@ActionedDate", eventLogEntry.ActionedDate);
                var pUserID = new SqlParameter("@UserID", userId);
                var pCustomerTime = new SqlParameter("@CustomerTime", dtNow);

                var result = await _context.Set<JsonResult>().FromSqlRaw("EXEC Custom_EventLog_Save_Log @EventLogID, @EventLogHeaderID, @LogEntryDateTime, " +
                    "@IncidentDetails, @SourceOfInformation, @IsConfirmed, @CMTAction, @ActionPriority,@ActionUser,@ActionGroup, " +
                    "@ActionDueBy,@StatusOfAction,@ActionDetail,@Comments,@ActionedDate,@UserID,@CustomerTime",
                        pEventLogID, pEventLogHeaderID, pLogEntryDateTime, pIncidentDetails, pSourceOfInformation, pIsConfirmed,
                        pCMTAction, pActionPriority, pActionUser, pActionGroup, pActionDueBy, pStatusOfAction,
                        pActionDetail, pComments, pActionedDate, pUserID, pCustomerTime).FirstOrDefaultAsync();
                if (result != null)
                {
                    return result.ResultId;
                }
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        public async Task<bool> SaveEventLogHeader(int activeIncidentId, int permittedDepartment, int companyId, int userId, string timeZoneId)
        {
            try
            {
                DateTimeOffset dtNow = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                var pActiveIncidentID = new SqlParameter("@activeIncidentId", activeIncidentId);
                var pPermittedDepartment = new SqlParameter("@permittedDepartment", permittedDepartment);
                var pCompanyID = new SqlParameter("@companyId", companyId);
                var pUserID = new SqlParameter("@userId", userId);
                var pCustomerTime = new SqlParameter("@CustomerTime", dtNow);

                await _context.Database.ExecuteSqlRawAsync("EXEC Custom_EventLog_SaveHeader @activeIncidentId, @permittedDepartment, @companyId, @userId, @CustomerTime",
                         pActiveIncidentID, pPermittedDepartment, pCompanyID, pUserID, pCustomerTime);
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public async Task<JsonResult> SaveLogMessage(int eventLogId, int messageId, string messageText, int userId, string timeZoneId)
        {
            try
            {
                DateTimeOffset dtNow = _DBC.GetDateTimeOffset(DateTime.Now, timeZoneId);

                var pEventLogID = new SqlParameter("@eventLogId", eventLogId);
                var pMessageID = new SqlParameter("@messageId", messageId);
                var pMessageText = new SqlParameter("@messageText", messageText);
                var pUserID = new SqlParameter("@userId", userId);
                var pCustomerTime = new SqlParameter("@CustomerTime", dtNow);

                var result = await _context.Set<JsonResult>().FromSqlRaw("EXEC Custom_Save_EventLog_Message @eventLogId, @messageId, @messageText, @userId, @CustomerTime",
                    pEventLogID, pMessageID, pMessageText, pUserID, pCustomerTime).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
