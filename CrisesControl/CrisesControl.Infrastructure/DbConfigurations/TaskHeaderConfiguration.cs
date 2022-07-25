using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskHeaderConfiguration : IEntityTypeConfiguration<TaskHeader>
{
    public void Configure(EntityTypeBuilder<TaskHeader> builder)
    {
        builder.ToTable("TaskHeader");

        builder.Property(e => e.TaskHeaderId).HasColumnName("TaskHeaderID");

        builder.Property(e => e.IncidentId).HasColumnName("IncidentID");

        builder.Property(e => e.ReviewFrequency).HasMaxLength(20);

        builder.Property(e => e.Rpo)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("RPO")
            .HasDefaultValueSql("((0))");

        builder.Property(e => e.Rto)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("RTO")
            .HasDefaultValueSql("((0))");
        builder.HasOne(x => x.IncidentActivation).WithMany().HasForeignKey(x => x.IncidentId);
    }
}