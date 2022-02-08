using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PublicAlertListConfiguration : IEntityTypeConfiguration<PublicAlertList>
{
    public void Configure(EntityTypeBuilder<PublicAlertList> builder)
    {
        builder.HasKey(e => e.ListId);

        builder.ToTable("PublicAlertList");

        builder.Property(e => e.ListId).HasColumnName("ListID");

        builder.Property(e => e.FileName).HasMaxLength(200);

        builder.Property(e => e.ListName).HasMaxLength(50);
    }
}