using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LanguageItemConfiguration : IEntityTypeConfiguration<LanguageItem>
{
    public void Configure(EntityTypeBuilder<LanguageItem> builder)
    {
        builder.ToTable("LanguageItem");

        builder.Property(e => e.LanguageItemId).HasColumnName("LanguageItemID");

        builder.Property(e => e.Comment).HasMaxLength(1000);

        builder.Property(e => e.ErrorCode).HasMaxLength(20);

        builder.Property(e => e.LangFile)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.LangKey)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Locale)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(e => e.ObjectType)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.Options).HasMaxLength(200);

        builder.Property(e => e.Title).HasMaxLength(200);
    }
}