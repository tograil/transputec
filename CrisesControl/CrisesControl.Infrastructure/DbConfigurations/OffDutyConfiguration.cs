using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class OffDutyConfiguration : IEntityTypeConfiguration<OffDuty>
{
    public void Configure(EntityTypeBuilder<OffDuty> builder)
    {
        builder.ToTable("OffDuty");

        builder.Property(e => e.OffDutyId).HasColumnName("OffDutyID");

        builder.Property(e => e.ActivationSource)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}