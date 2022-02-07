using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PublicAlertTemplateConfiguration : IEntityTypeConfiguration<PublicAlertTemplate>
{
    public void Configure(EntityTypeBuilder<PublicAlertTemplate> builder)
    {
        builder.HasKey(e => e.TemplateId);

        builder.ToTable("PublicAlertTemplate");

        builder.Property(e => e.TemplateId).HasColumnName("TemplateID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.FileName).HasMaxLength(200);

        builder.Property(e => e.TemplateName).HasMaxLength(150);
    }
}