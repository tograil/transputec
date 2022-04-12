using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.HasKey(e => e.OrderItemId)
            .HasName("PK_OrderItems");

        builder.Property(e => e.OrderItemId).HasColumnName("OrderItemID");

        builder.Property(e => e.Added)
            .HasMaxLength(5)
            .IsUnicode(false);

        builder.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.Discount).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.ModuleId).HasColumnName("ModuleID");

        builder.Property(e => e.OrderId).HasColumnName("OrderID");

        builder.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

        builder.ToTable("OrderDetails");
    }
}