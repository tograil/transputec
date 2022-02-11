using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TblTransferConfiguration : IEntityTypeConfiguration<TblTransfer>
{
    public void Configure(EntityTypeBuilder<TblTransfer> builder)
    {
        builder.ToTable("TblTransfer");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.CreatedDate)
            .HasColumnType("datetime")
            .HasDefaultValueSql("(getdate())");

        builder.Property(e => e.RefObject)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.SourceRefId).HasColumnName("SourceRefID");

        builder.Property(e => e.TargetRefId).HasColumnName("TargetRefID");
    }
}