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
        Task<IEnumerable<CompanyFtp>> GetCompanyFTP( int CompanyID);
        Task<IEnumerable<CascadingPlanReturn>> GetCascading(int PlanID, string PlanType, int CompanyId, bool GetDetails = false);
        List<CommsMethodPriority> GetCascadingDetails(int PlanID, int CompanyId);
        Task<List<CompanyParameterItem>> GetAllCompanyParameters(int companyId);
        Task<string> GetCompanyParameter(string Key, int CompanyId, string Default = "", string CustomerId = "");
        Task<Result> SaveCompanyFTP(int CompanyId, string HostName, string UserName, string SecurityKey, string Protocol,
                                 int Port, string RemotePath, string LogonType, bool DeleteSourceFile, string SHAFingerPrint);
        Task<bool> SaveCascading(int PlanID, string PlanName, string PlanType, bool LaunchSOS, int LaunchSOSInterval, List<CommsMethodPriority> CommsMethod, int CompanyID);
        Task<int> SaveCascadingPlanHeader(int PlanID, string PlanName, string PlanType, bool LaunchSOS, int LaunchSOSInterval, int CompanyID);
        Task SaveCascadingDetails(int PlanID, List<CommsMethodPriority> CommsMethod, int CompanyID);
        Task<bool> SavePriority(string ParamName, bool EnableSetting, List<CommsMethodPriority> CommsMethod, PriorityLevel PingPriority, PriorityLevel IncidentPriority,
            SeverityLevel IncidentSeverity, string Type, int UserID, int CompanyID, string TimeZoneId);
        Task UpdateCascadingAsync(int CompanyID);
        Task UpdateOffDuty(int CompanyID);
        Task<bool> CompanyDataReset(string[] ResetOptions, int CompanyID, string TimeZoneId);
        Task ResetGlobalConfig(int CompanyID, string TimeZoneId);
        Task ResetPings(int CompanyID);
        Task ResetActiveIncident(int CompanyID);
        Task<bool> DeleteCascading(int PlanID, int CompanyId, int UserId);
        Task<bool> SaveParameter(int ParameterID, string ParameterName, string ParameterValue, int CurrentUserID, int CompanyID, string TimeZoneId);
        Task<int> AddCompanyParameter(string Name, string Value, int CompanyId, int CurrentUserId, string TimeZoneId);
        Task SetSSOParameters(int CompanyId);
    }
}
