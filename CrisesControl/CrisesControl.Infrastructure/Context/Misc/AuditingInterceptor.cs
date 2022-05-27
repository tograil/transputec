using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CrisesControl.Core.AuditLog;
using CrisesControl.Core.AuditLog.Services;
using CrisesControl.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CrisesControl.Infrastructure.Context.Misc
{
    public class AuditingInterceptor : ISaveChangesInterceptor
    {
        private readonly IAuditLogService _auditLogService;
        private SaveChangesAudit _audit;

        public AuditingInterceptor(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _audit = CreateAudit(eventData.Context!);

            _auditLogService.AppendDataAudit(_audit);

            return new ValueTask<InterceptionResult<int>>(result);
        }

        public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            _audit = CreateAudit(eventData.Context!);

            _auditLogService.AppendDataAudit(_audit);

            return result;
        }

        public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            throw new System.NotImplementedException();
        }
        public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _audit.Succeeded = true;
            _audit.EndTime = DateTime.UtcNow;

            return new ValueTask<int>(result);
        }

        public void SaveChangesFailed(DbContextErrorEventData eventData)
        {
            _audit.Succeeded = false;
            _audit.EndTime = DateTime.UtcNow;
            _audit.ErrorMessage = eventData.Exception.Message;
        }

        public Task SaveChangesFailedAsync(DbContextErrorEventData eventData,
            CancellationToken cancellationToken = new CancellationToken())
        {
            _audit.Succeeded = false;
            _audit.EndTime = DateTime.UtcNow;
            _audit.ErrorMessage = eventData.Exception.Message;

            return Task.CompletedTask;
        }

        private static SaveChangesAudit CreateAudit(DbContext context)
        {
            context.ChangeTracker.DetectChanges();

            var audit = new SaveChangesAudit
            {
                AuditId = Guid.NewGuid(),
                StartTime = DateTime.UtcNow
            };

            foreach (var entry in context.ChangeTracker.Entries())
            {
                var (status, auditMessage) = entry.State switch
                {
                    EntityState.Deleted => CreateDeletedMessage(entry),
                    EntityState.Modified => CreateModifiedMessage(entry),
                    EntityState.Added => CreateAddedMessage(entry),
                    _ => (AuditLogType.Update, null)
                };

                if (auditMessage != null)
                {
                    audit.Entities.Add(new EntityAudit { State = status, AuditMessage = auditMessage });
                }
            }

            return audit;

            (AuditLogType, string) CreateAddedMessage(EntityEntry entry)
                => ( AuditLogType.Insert, entry.Properties.Aggregate(
                    $"Inserting {entry.Metadata.DisplayName()} with ",
                    (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' "));

            (AuditLogType, string) CreateModifiedMessage(EntityEntry entry)
                => (AuditLogType.Update, entry.Properties.Where(property => property.IsModified || property.Metadata.IsPrimaryKey()).Aggregate(
                    $"Updating {entry.Metadata.DisplayName()} with ",
                    (auditString, property) => auditString + $"{property.Metadata.Name}: Old Value: '{property.OriginalValue}' New Value: '{property.CurrentValue}' "));

            (AuditLogType, string) CreateDeletedMessage(EntityEntry entry)
                => (AuditLogType.Delete, entry.Properties.Where(property => property.Metadata.IsPrimaryKey()).Aggregate(
                    $"Deleting {entry.Metadata.DisplayName()} with ",
                    (auditString, property) => auditString + $"{property.Metadata.Name}: '{property.CurrentValue}' "));
        }
    }
}