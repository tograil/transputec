using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class OffDutyHistoryConfiguration : IEntityTypeConfiguration<OffDutyHistory>
{
    public void Configure(EntityTypeBuilder<OffDutyHistory> builder)
    {
        builder.HasKey(e => e.OffDutyId);

        builder.ToTable("OffDutyHistory");

        builder.Property(e => e.OffDutyId).HasColumnName("OffDutyID");

        builder.Property(e => e.ActivationSource)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}