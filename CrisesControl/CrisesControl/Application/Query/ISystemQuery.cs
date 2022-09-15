using CrisesControl.Api.Application.Commands.System.ApiStatus;
using CrisesControl.Api.Application.Commands.System.CleanLoadTestResult;
using CrisesControl.Api.Application.Commands.System.CompanyStatsAdmin;
using CrisesControl.Api.Application.Commands.System.DownloadExportFile;
using CrisesControl.Api.Application.Commands.System.ExportCompanyData;
using CrisesControl.Api.Application.Commands.System.ExportTrackingData;
using CrisesControl.Api.Application.Commands.System.GetAuditLogsByRecordId;
using CrisesControl.Api.Application.Commands.System.PushCMLog;
using CrisesControl.Api.Application.Commands.System.PushTwilioLog;
using CrisesControl.Api.Application.Commands.System.TwilioLogDump;
using CrisesControl.Api.Application.Commands.System.ViewErrorLog;
using CrisesControl.Api.Application.Commands.System.ViewModelLog;

namespace CrisesControl.Api.Application.Query
{
    public interface ISystemQuery
    {
        Task<ExportTrackingDataResponse> ExportTrackingData(ExportTrackingDataRequest request);
        Task<ViewModelLogResponse> ViewModelLog(ViewModelLogRequest request);
        Task<ViewErrorLogResponse> ViewErrorLog(ViewErrorLogRequest request);
        Task<ApiStatusResponse> ApiStatus(ApiStatusRequest request);
        Task<CompanyStatsAdminResponse> CompanyStatsAdmin(CompanyStatsAdminRequest request);
        Task<CleanLoadTestResultResponse> CleanLoadTestResult(CleanLoadTestResultRequest request);
        Task<GetAuditLogsByRecordIdResponse> GetAuditLogsByRecordId(GetAuditLogsByRecordIdRequest request);
        Task<TwilioLogDumpResponse> TwilioLogDump(TwilioLogDumpRequest request);
        Task<PushTwilioLogResponse> PushTwilioLog(PushTwilioLogRequest request);
        Task<PushCMLogResponse> PushCMLog(PushCMLogRequest request);
        Task<ExportCompanyDataResponse> ExportCompanyData(ExportCompanyDataRequest request);
        Task<DownloadExportFileResponse> DownloadExportFile(DownloadExportFileRequest request);
    }
}
