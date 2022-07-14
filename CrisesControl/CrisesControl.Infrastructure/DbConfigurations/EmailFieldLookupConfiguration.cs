using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class EmailFieldLookupConfiguration : IEntityTypeConfiguration<EmailFieldLookup>
{
    public void Configure(EntityTypeBuilder<EmailFieldLookup> builder)
    {
        builder.HasKey(e => e.FieldId);

        builder.ToTable("EmailFieldLookup");

        builder.Property(e => e.FieldId).HasColumnName("FieldID");

        builder.Property(e => e.FieldCode)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.FieldDescription).HasMaxLength(500);

        builder.Property(e => e.FieldName)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.FieldType).HasDefaultValueSql("((1))");

        builder.Property(e => e.SampleValue).HasMaxLength(250);

        builder.Property(e => e.ValidateField)
            .HasMaxLength(250)
            .IsUnicode(false);

     
    }
}