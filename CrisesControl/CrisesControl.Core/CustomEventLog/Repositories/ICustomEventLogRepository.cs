using CrisesControl.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CustomEventLog.Repositories
{
    public interface ICustomEventLogRepository
    {
        Task<EventLogListing> GetEventLog(int eventLogId, int eventLogHeaderId, int companyId, int userId);
        Task<EventLogHeader> GetEventLogHeader(int activeIncidentId, int eventLogHeaderId, int companyId, int userId);
        Task<List<EventLogListing>> GetLogs(int activeIncidentId, int eventLogHeaderId, int companyId, int userId);
        Task<List<EventMessageLog>> GetMessageLog(int eventLogId, int companyId, int userId);
        Task<bool> SaveEventLogHeader(int activeIncidentId, int permittedDepartment, int companyId, int userId, string timeZoneId);
        Task<int> SaveEventLog(EventLogEntry eventLogEntry, int userId, string timeZoneId);
        Task<JsonResult> SaveLogMessage(int eventLogId, int messageId, string messageText, int userId, string timeZoneId);
        Task<ExportLogResponse> ExportEventLog(EventLogModel eventLogModel);
    }
}
