using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TwilioLogBatchConfiguration : IEntityTypeConfiguration<TwilioLogBatch>
{
    public void Configure(EntityTypeBuilder<TwilioLogBatch> builder)
    {
        builder.HasKey(e => e.BatchId);

        builder.ToTable("TwilioLogBatch");

        builder.Property(e => e.CommsProvider).HasMaxLength(25);

        builder.Property(e => e.LogType)
            .HasMaxLength(20)
            .IsUnicode(false);
    }
}