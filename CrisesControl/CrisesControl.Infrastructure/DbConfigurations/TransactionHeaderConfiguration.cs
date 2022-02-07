using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TransactionHeaderConfiguration : IEntityTypeConfiguration<TransactionHeader>
{
    public void Configure(EntityTypeBuilder<TransactionHeader> builder)
    {
        builder.ToTable("TransactionHeader");

        builder.Property(e => e.CreditBalance).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.CreditLimit).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.NetTotal).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.StatementDate).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.Total).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.VatRate).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Vatvalue)
            .HasColumnType("decimal(20, 4)")
            .HasColumnName("VATValue");
    }
}