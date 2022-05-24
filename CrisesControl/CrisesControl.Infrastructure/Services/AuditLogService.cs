using System.Collections.Generic;
using CrisesControl.Core.AuditLog;
using CrisesControl.Core.AuditLog.Services;
using CrisesControl.SharedKernel.Enums;

namespace CrisesControl.Infrastructure.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly List<SaveChangesAudit> _changes = new List<SaveChangesAudit>();

        public void AppendDataAudit(SaveChangesAudit saveChangesAudit)
        {
            _changes.Add(saveChangesAudit);
        }

        public IReadOnlyCollection<SaveChangesAudit> GetSaveChangesAudit()
        {
            return _changes;
        }
    }
}