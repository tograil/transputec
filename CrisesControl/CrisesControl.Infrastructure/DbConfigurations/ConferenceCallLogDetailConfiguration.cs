using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ConferenceCallLogDetailConfiguration : IEntityTypeConfiguration<ConferenceCallLogDetail>
{
    public void Configure(EntityTypeBuilder<ConferenceCallLogDetail> builder)
    {
        builder.HasKey(e => e.ConferenceCallDetailId)
            .HasName("PK_dbo.ConferenceCallLogDetail");

        builder.ToTable("ConferenceCallLogDetail");

        builder.Property(e => e.CalledOn).HasMaxLength(10);

        builder.Property(e => e.ConfJoined).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.ConfLeft).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.Landline).HasMaxLength(20);

        builder.Property(e => e.PhoneNumber).HasMaxLength(20);

        builder.Property(e => e.Status).HasMaxLength(50);

        builder.Property(e => e.SuccessCallId).HasMaxLength(50);
    }
}