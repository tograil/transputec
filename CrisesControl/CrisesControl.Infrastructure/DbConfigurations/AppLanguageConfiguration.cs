using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AppLanguageConfiguration : IEntityTypeConfiguration<AppLanguage>
{
    public void Configure(EntityTypeBuilder<AppLanguage> builder)
    {

        builder.HasKey(e => e.LanguageId);

        builder.ToTable("AppLanguage");

        builder.Property(e => e.LanguageId).HasColumnName("LanguageID");

        builder.Property(e => e.FlagIcon)
                .HasMaxLength(250)
                .IsUnicode(false);

        builder.Property(e => e.IconFolder)
                .HasMaxLength(50)
                .IsUnicode(false);

        builder.Property(e => e.IconUrl)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("IconURL");

        builder.Property(e => e.LanguageName)
                .HasMaxLength(50)
                .IsUnicode(false);

        builder.Property(e => e.Locale)
                .HasMaxLength(10)
                .IsUnicode(false);

        builder.Property(e => e.Platform)
                .HasMaxLength(50)
                .IsUnicode(false);
    }
}