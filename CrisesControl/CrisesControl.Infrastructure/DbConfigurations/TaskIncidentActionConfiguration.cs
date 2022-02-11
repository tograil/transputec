using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskIncidentActionConfiguration : IEntityTypeConfiguration<TaskIncidentAction>
{
    public void Configure(EntityTypeBuilder<TaskIncidentAction> builder)
    {
        builder.HasKey(e => e.IncidentTaskActionId)
            .HasName("PK_dbo.TaskIncidentAction");

        builder.ToTable("TaskIncidentAction");

        builder.Property(e => e.IncidentTaskActionId).HasColumnName("IncidentTaskActionID");

        builder.Property(e => e.ActiveIncidentTaskId).HasColumnName("ActiveIncidentTaskID");

        builder.Property(e => e.TaskActionTypeId).HasColumnName("TaskActionTypeID");
    }
}