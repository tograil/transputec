using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyTransactionTypeConfiguration : IEntityTypeConfiguration<CompanyTransactionType>
{
    public void Configure(EntityTypeBuilder<CompanyTransactionType> builder)
    {
        builder.ToTable("CompanyTranscationType");

        builder.HasNoKey();

        builder.Property(e => e.NextRunDate).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.PaymentMethod).HasMaxLength(20);

        builder.Property(e => e.PaymentPeriod).HasMaxLength(20);

        builder.Property(e => e.TransactionRate).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");
    }
}