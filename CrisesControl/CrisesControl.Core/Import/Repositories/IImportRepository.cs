using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisesControl.Core.Import.Repositories
{
    public interface IImportRepository
    {
        public Task<dynamic> DepartmentOnlyImport(GroupOnlyImportModel groupOnlyImportModel, CancellationToken cancellationToken);
        public Task<CommonDTO> DepartmentOnlyUpload(GroupOnlyUploadModel groupOnlyUploadModel, CancellationToken cancellationToken);
        public List<ImportDumpResult> DownloadImportResult(int companyId, int sessionId, CancellationToken cancellationToken);
        public Task<List<ImportDumpResult>> GetImportResult(ProcessImport processImport, CancellationToken cancellationToken);
        public Task<List<ImportDumpResult>> GetValidationResult(ProcessImport processImport, CancellationToken cancellationToken);
        public Task<dynamic> GroupOnlyImport(GroupOnlyImportModel groupOnlyImportModel, CancellationToken cancellationToken);
        public Task<CommonDTO> GroupOnlyUpload(GroupOnlyUploadModel groupOnlyUploadModel, CancellationToken cancellationToken);
        public Task<dynamic> LocationOnlyImport(LocationOnlyImportModel locationOnlyImportModel, CancellationToken cancellationToken);
        public Task<CommonDTO> LocationOnlyUpload(LocationOnlyUploadModel locationOnlyUploadModel, CancellationToken cancellationToken);
        public Task<bool> ProcessUserImport(ProcessImport processImport, CancellationToken cancellationToken);
        public dynamic QueueImportJob(QueueImport queueImport, CancellationToken cancellationToken);
        public void UploadSingleFile();
        public void UserCompleteImport(UserCompleteImportModel userCompleteImportModel, CancellationToken cancellationToken);
        public void UserCompleteUpload(UserCompleteUploadModel userCompleteUploadModel, CancellationToken cancellationToken);
        public bool CreateTempDepartment(List<ImportDumpInput> data, string sessionId, int companyId, int userId, string timeZoneId);
        public bool CreateTempUsers(List<ImportDumpInput> userData, string SessionId, int CompanyId, string JobType, int UserId = 0, string TimeZoneId = "GMT Standard Time");
        Task<CommonDTO> RefreshTmpTable(int companyId, int userId, string sessionId);
        public bool CreateTempLocation(List<ImportDumpInput> locData, string sessionId, int companyId, int userId = 0, string timeZoneId = "GMT Standard Time");
        public bool CreateTempGroup(List<ImportDumpInput> data, string sessionId, int companyId, int userId, string timeZoneId);
    }
}
