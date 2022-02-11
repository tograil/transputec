using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ImportDumpHeaderConfiguration : IEntityTypeConfiguration<ImportDumpHeader>
{
    public void Configure(EntityTypeBuilder<ImportDumpHeader> builder)
    {
        builder.ToTable("ImportDumpHeader");

        builder.Property(e => e.FileName).HasMaxLength(250);

        builder.Property(e => e.ImportTriggerId).HasColumnName("ImportTriggerID");

        builder.Property(e => e.JobType).HasMaxLength(20);

        builder.Property(e => e.MappingFileName).HasMaxLength(250);

        builder.Property(e => e.SendInvite).HasDefaultValueSql("((0))");

        builder.Property(e => e.SessionId).HasMaxLength(250);

        builder.Property(e => e.Status).HasMaxLength(50);
    }
}