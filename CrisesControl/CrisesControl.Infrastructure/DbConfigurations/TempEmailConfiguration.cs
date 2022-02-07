using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TempEmailConfiguration : IEntityTypeConfiguration<TempEmail>
{
    public void Configure(EntityTypeBuilder<TempEmail> builder)
    {
        builder.HasNoKey();

        builder.ToTable("TempEmail");

        builder.Property(e => e.Code)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.Description).HasMaxLength(250);

        builder.Property(e => e.EmailSubject).HasMaxLength(200);

        builder.Property(e => e.Locale)
            .HasMaxLength(5)
            .IsUnicode(false);

        builder.Property(e => e.Name).HasMaxLength(150);

        builder.Property(e => e.Type)
            .HasMaxLength(20)
            .IsUnicode(false);
    }
}