using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskIncidentPredecessorConfiguration : IEntityTypeConfiguration<TaskIncidentPredecessor>
{
    public void Configure(EntityTypeBuilder<TaskIncidentPredecessor> builder)
    {
        builder.HasKey(e => e.TaskPredecessorId)
            .HasName("PK_dbo.TaskIncidentPredecessor");

        builder.ToTable("TaskIncidentPredecessor");

        builder.HasIndex(e => e.IncidentTaskId, "IDX_IncidentTaskID");

        builder.HasIndex(e => e.PredecessorTaskId, "IDX_PredecessorTaskID");

        builder.Property(e => e.TaskPredecessorId).HasColumnName("TaskPredecessorID");

        builder.Property(e => e.IncidentTaskId).HasColumnName("IncidentTaskID");

        builder.Property(e => e.PredecessorTaskId).HasColumnName("PredecessorTaskID");
    }
}