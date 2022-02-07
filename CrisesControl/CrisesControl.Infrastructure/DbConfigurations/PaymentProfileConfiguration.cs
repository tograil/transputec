using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PaymentProfileConfiguration : IEntityTypeConfiguration<PaymentProfile>
{
    public void Configure(EntityTypeBuilder<PaymentProfile> builder)
    {
        builder.ToTable("PaymentProfile");

        builder.Property(e => e.ConfUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.CreditBalance).HasColumnType("decimal(18, 4)");

        builder.Property(e => e.CreditLimit).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.EmailUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumBalance).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumConfRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumEmailRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumPhoneRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumPushRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumTextRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.PhoneUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.PushUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.SoptokenValue)
            .HasColumnType("decimal(20, 4)")
            .HasColumnName("SOPTokenValue");

        builder.Property(e => e.TextUplift).HasColumnType("decimal(20, 4)");
    }
}