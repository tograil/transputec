using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskActiveIncidentConfiguration : IEntityTypeConfiguration<TaskActiveIncident>
{
    public void Configure(EntityTypeBuilder<TaskActiveIncident> builder)
    {
        builder.HasKey(e => e.ActiveIncidentTaskId)
            .HasName("PK_dbo.TaskActiveIncident");

        builder.ToTable("TaskActiveIncident");

        builder.Property(e => e.ActiveIncidentTaskId).HasColumnName("ActiveIncidentTaskID");

        builder.Property(e => e.ActiveIncidentId).HasColumnName("ActiveIncidentID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.IncidentTaskId).HasColumnName("IncidentTaskID");

        builder.Property(e => e.NextIncidentTaskId).HasColumnName("NextIncidentTaskID");

        builder.Property(e => e.PreviousIncidentTaskId).HasColumnName("PreviousIncidentTaskID");

        builder.Property(e => e.PreviousOwnerId).HasColumnName("PreviousOwnerID");

        builder.Property(e => e.TaskOwnerId).HasColumnName("TaskOwnerID");

        builder.Property(e => e.TaskTitle).HasMaxLength(250);
    }
}