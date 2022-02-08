using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibMessageResponseConfiguration : IEntityTypeConfiguration<LibMessageResponse>
{
    public void Configure(EntityTypeBuilder<LibMessageResponse> builder)
    {
        builder.HasKey(e => e.LibResponseId)
            .HasName("PK_dbo.LibMessageResponse");

        builder.ToTable("LibMessageResponse");

        builder.Property(e => e.LibResponseId).HasColumnName("LibResponseID");

        builder.Property(e => e.Description).HasMaxLength(500);

        builder.Property(e => e.MessageType)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.ResponseLabel).HasMaxLength(50);
    }
}