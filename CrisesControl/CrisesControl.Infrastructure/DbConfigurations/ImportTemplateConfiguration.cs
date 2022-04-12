using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ImportTemplateConfiguration : IEntityTypeConfiguration<ImportTemplate>
{
    public void Configure(EntityTypeBuilder<ImportTemplate> builder)
    {
        builder.HasKey(e => e.TemplateId);

        builder.Property(e => e.TemplateId).HasColumnName("TemplateID");

        builder.Property(e => e.TemplateFile)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.TemplateName).HasMaxLength(100);

        builder.Property(e => e.TemplateType)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.ToTable("ImportTemplates");
    }
}