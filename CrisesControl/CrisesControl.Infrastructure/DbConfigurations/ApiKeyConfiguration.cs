using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKey");

        builder.Property(e => e.ApikeyId).HasColumnName("APIKeyId");

        builder.Property(e => e.AllowedIp)
                .HasMaxLength(20)
                .HasColumnName("AllowedIP");

        builder.Property(e => e.Apikey1)
                .HasMaxLength(100)
                .HasColumnName("APIKey");

        builder.Property(e => e.Description).HasMaxLength(250);
    }
}