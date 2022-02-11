using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskActiveIncidentPredecessorConfiguration : IEntityTypeConfiguration<TaskActiveIncidentPredecessor>
{
    public void Configure(EntityTypeBuilder<TaskActiveIncidentPredecessor> builder)
    {
        builder.HasKey(e => e.TaskPredecessorId)
            .HasName("PK_dbo.TaskActiveIncidentPredecessor");

        builder.ToTable("TaskActiveIncidentPredecessor");

        builder.Property(e => e.TaskPredecessorId).HasColumnName("TaskPredecessorID");

        builder.Property(e => e.ActiveIncidentTaskId).HasColumnName("ActiveIncidentTaskID");

        builder.Property(e => e.PredecessorTaskId).HasColumnName("PredecessorTaskID");
    }
}