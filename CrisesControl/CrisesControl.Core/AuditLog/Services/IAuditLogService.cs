using System.Collections.Generic;
using System.Threading.Tasks;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Core.AuditLog.Services
{
    public interface IAuditLogService
    {
        void AppendDataAudit(SaveChangesAudit saveChangesAudit);

        IReadOnlyCollection<SaveChangesAudit> GetSaveChangesAudit();

        Task SaveAuditData(AuditLogEntry auditLogEntry);
    }
}