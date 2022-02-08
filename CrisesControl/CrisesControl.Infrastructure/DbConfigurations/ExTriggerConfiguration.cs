using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ExTriggerConfiguration : IEntityTypeConfiguration<ExTrigger>
{
    public void Configure(EntityTypeBuilder<ExTrigger> builder)
    {
        builder.ToTable("ExTrigger");

        builder.HasIndex(e => e.CompanyId, "IX_CompanyId");

        builder.Property(e => e.ExTriggerId).HasColumnName("ExTriggerID");

        builder.Property(e => e.ActionType).HasMaxLength(50);

        builder.Property(e => e.ColumnMappingFilePath).HasMaxLength(500);

        builder.Property(e => e.ColumnMappingFileType).HasMaxLength(10);

        builder.Property(e => e.CommandLine).HasMaxLength(150);

        builder.Property(e => e.Delimiter).HasMaxLength(2);

        builder.Property(e => e.ImportFileType).HasMaxLength(10);

        builder.Property(e => e.ImportSource).HasMaxLength(10);

        builder.Property(e => e.JobDescription).HasMaxLength(500);

        builder.Property(e => e.JobIncidentId).HasColumnName("JobIncidentID");

        builder.Property(e => e.JobKey).HasMaxLength(100);

        builder.Property(e => e.JobName).HasMaxLength(100);

        builder.Property(e => e.JobType).HasMaxLength(20);

        builder.Property(e => e.Smskey)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("SMSKey");

        builder.Property(e => e.SourceEmail).HasMaxLength(150);

        builder.Property(e => e.SourceFilePath).HasMaxLength(500);

        builder.Property(e => e.SourceNumber).HasMaxLength(20);

        builder.Property(e => e.SourceNumberIsd)
            .HasMaxLength(8)
            .HasColumnName("SourceNumberISD");
    }
}