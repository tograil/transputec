using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TransactionTypeConfiguration : IEntityTypeConfiguration<TransactionType>
{
    public void Configure(EntityTypeBuilder<TransactionType> builder)
    {
        builder.ToTable("TransactionType");

        builder.Property(e => e.TransactionTypeId).HasColumnName("TransactionTypeID");

        builder.Property(e => e.ChargeType).HasMaxLength(15);

        builder.Property(e => e.Rate).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.TransactionCode).HasMaxLength(100);

        builder.Property(e => e.TransactionDescription).HasMaxLength(250);

        builder.Property(e => e.TransactionTypeName).HasMaxLength(100);
    }
}