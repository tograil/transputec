using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AttachmentTypeConfiguration : IEntityTypeConfiguration<AttachmentType>
{
    public void Configure(EntityTypeBuilder<AttachmentType> builder)
    {
        builder.HasKey(e => e.TypeId);

        builder.ToTable("AttachmentType");

        builder.Property(e => e.TypeId).HasColumnName("TypeID");

        builder.Property(e => e.TypeName)
                .HasMaxLength(20)
                .IsUnicode(false);
    }
}