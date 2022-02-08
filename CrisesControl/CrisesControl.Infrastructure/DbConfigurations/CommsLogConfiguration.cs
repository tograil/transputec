using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CommsLogConfiguration : IEntityTypeConfiguration<CommsLog>
{
    public void Configure(EntityTypeBuilder<CommsLog> builder)
    {
        builder.HasKey(e => e.LogId)
                .HasName("PK_dbo.CommsLogs");

        builder.HasIndex(e => e.CommType, "IDX_CommsType");

        builder.HasIndex(e => e.Sid, "IDX_SId");

        builder.HasIndex(e => e.Status, "IDX_Status");

        builder.Property(e => e.AnsweredBy).HasMaxLength(20);

        builder.Property(e => e.CommType).HasMaxLength(10);

        builder.Property(e => e.CommsProvider)
                .HasMaxLength(25)
                .IsUnicode(false);

        builder.Property(e => e.Direction).HasMaxLength(15);

        builder.Property(e => e.ErrorCode).HasMaxLength(15);

        builder.Property(e => e.FromFormatted).HasMaxLength(20);

        builder.Property(e => e.Price).HasColumnType("decimal(20, 4)");

        builder.Property(e => e.PriceUnit).HasMaxLength(3);

        builder.Property(e => e.Sid).HasMaxLength(100);

        builder.Property(e => e.Status).HasMaxLength(50);

        builder.Property(e => e.ToFormatted).HasMaxLength(200);
    }
}