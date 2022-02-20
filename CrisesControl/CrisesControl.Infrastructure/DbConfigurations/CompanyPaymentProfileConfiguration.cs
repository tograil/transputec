using CrisesControl.Core.CompanyAggregate;
using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyPaymentProfileConfiguration : IEntityTypeConfiguration<CompanyPaymentProfile>
{
    public void Configure(EntityTypeBuilder<CompanyPaymentProfile> builder)
    {
        builder.ToTable("CompanyPaymentProfile");

        builder.Property(e => e.AgreementNo).HasMaxLength(50);

        builder.Property(e => e.BillingAddress1).HasMaxLength(150);

        builder.Property(e => e.BillingAddress2).HasMaxLength(150);

        builder.Property(e => e.BillingEmail).HasMaxLength(150);

        builder.Property(e => e.CardExpiryDate).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.CardHolderName).HasMaxLength(150);

        builder.Property(e => e.CardType).HasMaxLength(50);

        builder.Property(e => e.City).HasMaxLength(50);

        builder.Property(e => e.ConfUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.Country).HasMaxLength(150);

        builder.Property(e => e.CreditBalance).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.CreditLimit).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.CurrentStatementEndDate).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.EmailUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.Ipaddress)
            .HasMaxLength(20)
            .HasColumnName("IPAddress");

        builder.Property(e => e.LastStatementEndDate).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.MaxTransactionLimit).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.MinimumBalance).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumConfRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumEmailRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumPhoneRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumPushRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumTextRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.PaymentPeriod).HasMaxLength(10);

        builder.Property(e => e.PhoneUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.Postcode).HasMaxLength(20);

        builder.Property(e => e.PushUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.SoptokenValue)
            .HasColumnType("decimal(20, 4)")
            .HasColumnName("SOPTokenValue");

        builder.Property(e => e.TextUplift).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.Town).HasMaxLength(50);

        builder.Property(e => e.Vatrate)
            .HasColumnType("decimal(18, 2)")
            .HasColumnName("VATRate")
            .HasDefaultValueSql("((20))");
    }
}