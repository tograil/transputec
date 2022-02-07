using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ObjectConfiguration : IEntityTypeConfiguration<Object>
{
    public void Configure(EntityTypeBuilder<Object> builder)
    {
        builder.HasIndex(e => e.ObjectName, "IDX_ObjectName")
            .HasFillFactor(100);

        builder.Property(e => e.ObjectId).HasColumnName("ObjectID");

        builder.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

        builder.Property(e => e.ObjectName).HasMaxLength(50);

        builder.Property(e => e.ObjectTableName).HasMaxLength(50);

        builder.Property(e => e.UpdatedOn).HasDefaultValueSql("(getdate())");
    }
}