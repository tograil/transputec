using CrisesControl.Core.Companies;
using CrisesControl.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrisesControl.Core.CompanyParameters.Repositories
{
    public interface ICompanyParametersRepository
    {
        Task<IEnumerable<CompanyFtp>> GetCompanyFTP( int companyID);
        Task<IEnumerable<CascadingPlanReturn>> GetCascading(int planID, string planType, int companyId, bool getDetails = false);
        List<CommsMethodPriority> GetCascadingDetails(int planID, int companyId);
        Task<List<CompanyParameterItem>> GetAllCompanyParameters(int companyId);
        Task<string> GetCompanyParameter(string key, int companyId, string Default = "", string customerId = "");
        Task<Result> SaveCompanyFTP(int companyId, string hostName, string userName, string SecurityKey, string protocol,
                                 int port, string remotePath, string logonType, bool deleteSourceFile, string shaFingerPrint);
        Task<bool> SaveCascading(int planID, string planName, string planType, bool launchSOS, int launchSOSInterval, List<CommsMethodPriority> commsMethod, int companyID);
        Task<int> SaveCascadingPlanHeader(int planID, string planName, string planType, bool launchSOS, int launchSOSInterval, int companyID);
        Task SaveCascadingDetails(int planID, List<CommsMethodPriority> commsMethod, int companyID);
        Task<bool> SavePriority(string paramName, bool enableSetting, List<CommsMethodPriority> commsMethod, PriorityLevel pingPriority, PriorityLevel incidentPriority,
            SeverityLevel incidentSeverity, string type, int userID, int companyID, string timeZoneId);
        void UpdateCascadingAsync(int companyID);
        void UpdateOffDuty(int companyID);
        Task<bool> CompanyDataReset(string[] ResetOptions, int companyID, string timeZoneId);
        void ResetGlobalConfig(int companyID, string timeZoneId);
        void ResetPings(int companyID);
        void ResetActiveIncident(int companyID);
        Task<bool> DeleteCascading(int planID, int companyId, int userId);
        Task<bool> SaveParameter(int parameterID, string parameterName, string parameterValue, int currentUserID, int companyID, string timeZoneId);
        Task<int> AddCompanyParameter(string name, string value, int companyId, int currentUserId, string timeZoneId);
        void SetSSOParameters(int companyId);
        Task<OTPResponse> SegregationOtp(int companyId, int currentUserId, string method);
    

    }
}
