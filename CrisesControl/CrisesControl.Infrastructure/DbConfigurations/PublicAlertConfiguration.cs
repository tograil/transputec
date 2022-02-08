using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PublicAlertConfiguration : IEntityTypeConfiguration<PublicAlert>
{
    public void Configure(EntityTypeBuilder<PublicAlert> builder)
    {
        builder.ToTable("PublicAlert");

        builder.Property(e => e.PublicAlertId).HasColumnName("PublicAlertID");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");
    }
}