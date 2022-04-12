using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TransactionDetailConfiguration : IEntityTypeConfiguration<TransactionDetail>
{
    public void Configure(EntityTypeBuilder<TransactionDetail> builder)
    {
        builder.HasKey(e => e.TransactionDetailsId)
            .HasName("PK_dbo.TransactionDetails");

        builder.Property(e => e.Cost).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.Drcr)
            .HasMaxLength(2)
            .HasColumnName("DRCR");

        builder.Property(e => e.LineValue).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.LineVat).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.MinimumPrice).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.Total).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.TransactionRate).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.TransactionReference).HasMaxLength(150);

        builder.ToTable("TransactionDetails");
    }
}