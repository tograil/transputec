using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.HasKey(e => e.TemplateId);

        builder.ToTable("EmailTemplate");

        builder.Property(e => e.TemplateId).HasColumnName("TemplateID");

        builder.Property(e => e.Code)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.Description).HasMaxLength(250);

        builder.Property(e => e.EmailSubject).HasMaxLength(200);

        builder.Property(e => e.Locale)
            .HasMaxLength(5)
            .IsUnicode(false);

        builder.Property(e => e.Name).HasMaxLength(150);

        builder.Property(e => e.Status).HasDefaultValueSql("((1))");

        builder.Property(e => e.Type)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.UpdatedOn).HasDefaultValueSql("(getdate())");
    }
}