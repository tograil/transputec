using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TwilioLogPushHostConfiguration : IEntityTypeConfiguration<TwilioLogPushHost>
{
    public void Configure(EntityTypeBuilder<TwilioLogPushHost> builder)
    {
        builder.ToTable("TwilioLogPushHost");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.ApiHost).HasMaxLength(20);

        builder.Property(e => e.LogCollectionUrl)
            .HasMaxLength(200)
            .HasColumnName("LogCollectionURL");
    }
}