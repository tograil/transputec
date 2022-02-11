using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class EmailTmplFieldMappingConfiguration : IEntityTypeConfiguration<EmailTmplFieldMapping>
{
    public void Configure(EntityTypeBuilder<EmailTmplFieldMapping> builder)
    {
        builder.HasNoKey();

        builder.ToTable("EmailTmplFieldMapping");

        builder.Property(e => e.FieldId).HasColumnName("FieldID");

        builder.Property(e => e.TemplateCode)
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}