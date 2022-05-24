using System.Collections.Generic;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Core.AuditLog.Services
{
    public interface IAuditLogService
    {
        void AppendDataAudit(SaveChangesAudit saveChangesAudit);

        IReadOnlyCollection<SaveChangesAudit> GetSaveChangesAudit();
    }
}