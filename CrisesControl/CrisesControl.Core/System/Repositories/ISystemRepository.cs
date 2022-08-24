using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace CrisesControl.Core.System.Repositories
{
    public  interface ISystemRepository
    {
        Task<string> ExportTrackingData(int trackMeID, int userDeviceID, DateTimeOffset startDate, DateTimeOffset endDate, int outUserCompanyId);
        Task<List<ModelLogReturn>> GetModelLog(DateTimeOffset startDate, DateTimeOffset endDate, int recordStart, int recordLength, string searchString, string orderBy, string orderDir);
        Task<HttpResponseMessage> DownloadExportFile(int companyId, string fileName);
        Task<bool> TwilioLogDump(string logType, List<CallResource> calls, List<MessageResource> texts, List<RecordingResource> recordings);
        Task<bool> PushTwilioLog(string method, string sId);
        Task<bool> PushCMLog(string method, string sId);
        Task<HttpResponseMessage> ApiStatus();
        Task CleanLoadTestResult();
        Task<List<ErrorLogReturn>> GetErrorLog(DateTimeOffset startDate, DateTimeOffset endDate, int recordStart, int recordLength, string searchString, string orderBy, string orderDir);
        Task<string> ExportCompanyData(int outUserCompanyId, string entity, int outLoginUserId, bool showDeleted = false);
        Task<List<AuditHelp>> GetAuditLogsByRecordId(string tableName, int recordId, bool isThisWeek, bool isThisMonth, bool isLastMonth,
           DateTimeOffset startDate, DateTimeOffset endDate, bool limitResult, int companyId);
        Task<HttpResponseMessage> CompanyStatsAdmin(int outUserCompanyId);
    }
}
