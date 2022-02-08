using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class OrderHeaderConfiguration : IEntityTypeConfiguration<OrderHeader>
{
    public void Configure(EntityTypeBuilder<OrderHeader> builder)
    {
        builder.HasKey(e => e.OrderId)
            .HasName("PK_Order");

        builder.ToTable("OrderHeader");

        builder.Property(e => e.OrderId).HasColumnName("OrderID");

        builder.Property(e => e.ContractStartDate).HasColumnType("datetime");

        builder.Property(e => e.ContractType)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.ContractValue).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.CustomerId).HasMaxLength(50);

        builder.Property(e => e.Discount).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.InvoiceNumber)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.NetTotal).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.PaymentMethod)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.Status)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.TigerOrderNo)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.VatTotal)
            .HasColumnType("decimal(18, 2)")
            .HasDefaultValueSql("((-1))");
    }
}