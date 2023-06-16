using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MonthlyTransactionItemConfiguration : IEntityTypeConfiguration<MonthlyTransactionItem>
{
    public void Configure(EntityTypeBuilder<MonthlyTransactionItem> builder)
    {
        builder.HasKey(e => e.TransactionId)
            .HasName("PK_dbo.MonthlyTransactionItems");

        builder.Property(e => e.TransactionId).HasColumnName("TransactionID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.ItemValue).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");

        builder.Property(e => e.UserId).HasColumnName("UserID");

        builder.Property(e => e.UserRole).HasMaxLength(20);

        builder.ToTable("MonthlyTransactionItems");
    }
}