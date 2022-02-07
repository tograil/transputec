using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachment");

        builder.Property(e => e.AttachmentId).HasColumnName("AttachmentID");

        builder.Property(e => e.AttachmentType)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.FileName).HasMaxLength(150);

        builder.Property(e => e.FileSize).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.MimeType).HasMaxLength(100);

        builder.Property(e => e.ObjectId).HasColumnName("ObjectID");

        builder.Property(e => e.OrigFileName).HasMaxLength(150);

        builder.Property(e => e.Title).HasMaxLength(250);
    }
}