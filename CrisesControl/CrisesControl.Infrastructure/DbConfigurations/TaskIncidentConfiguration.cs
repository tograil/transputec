using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskIncidentConfiguration : IEntityTypeConfiguration<TaskIncident>
{
    public void Configure(EntityTypeBuilder<TaskIncident> builder)
    {
        builder.HasKey(e => e.IncidentTaskId)
            .HasName("PK_dbo.TaskIncident");

        builder.ToTable("TaskIncident");

        builder.HasIndex(e => e.IncidentId, "IDX_IncidentID");

        builder.HasIndex(e => e.TaskHeaderId, "IDX_TaskHeaderID");

        builder.Property(e => e.IncidentTaskId).HasColumnName("IncidentTaskID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.IncidentId).HasColumnName("IncidentID");

        builder.Property(e => e.TaskHeaderId).HasColumnName("TaskHeaderID");

        builder.Property(e => e.TaskTitle).HasMaxLength(250);
    }
}