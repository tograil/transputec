using CrisesControl.Api.Application.Commands.CompanyParameters.GetCascading;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetCompanyFTP;
using CrisesControl.Api.Application.Commands.CompanyParameters.GetAllCompanyParameters;
using CrisesControl.Api.Application.Commands.CompanyParameters.SaveCompanyFTP;
using CrisesControl.Core.CompanyParameters;
using CrisesControl.Core.Companies;

namespace CrisesControl.Api.Application.Query
{
    public interface ICompanyParametersQuery
    {
        public Task<GetCascadingResponse> GetCascading(GetCascadingRequest request);
        public Task<GetCompanyFTPResponse> GetCompanyFTP(GetCompanyFTPRequest request);
        public Task<GetAllCompanyParametersResponse> GetAllCompanyParameters(GetAllCompanyParametersRequest request);
        Task<SaveCompanyFTPResponse> SaveCompanyFTP(SaveCompanyFTPRequest request);
        Task<bool> SaveCascading(int PlanID, string PlanName, string PlanType, bool LaunchSOS, int LaunchSOSInterval, List<CommsMethodPriority> CommsMethod, int CompanyID, int UserID);
        Task<int> SaveCascadingPlanHeader(int PlanID, string PlanName, string PlanType, bool LaunchSOS, int LaunchSOSInterval, int CompanyID, int UserID);
        Task SaveCascadingDetails(int PlanID, List<CommsMethodPriority> CommsMethod, int CompanyID);
        Task<int> SaveCascadingPlanHeader(int PlanID, string PlanName, string PlanType, bool LaunchSOS, int LaunchSOSInterval, int CompanyID);
        Task<bool> SavePriority(string ParamName, bool EnableSetting, List<CommsMethodPriority> CommsMethod, PriorityLevel PingPriority, PriorityLevel IncidentPriority,
            SeverityLevel IncidentSeverity, string Type, int UserID, int CompanyID, string TimeZoneId);
        Task UpdateCascadingAsync(int CompanyID);
        Task UpdateOffDuty(int CompanyID);
        Task<bool> CompanyDataReset(string[] ResetOptions, int CompanyID, string TimeZoneId);
        Task ResetGlobalConfig(int CompanyID, string TimeZoneId);
        Task ResetPings(int CompanyID);
        Task ResetActiveIncident(int CompanyID);
    }
}
