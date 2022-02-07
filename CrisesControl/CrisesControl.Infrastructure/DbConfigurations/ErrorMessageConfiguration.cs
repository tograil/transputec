using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ErrorMessageConfiguration : IEntityTypeConfiguration<ErrorMessage>
{
    public void Configure(EntityTypeBuilder<ErrorMessage> builder)
    {
        builder.HasKey(e => e.ErrorId)
            .HasName("PK_dbo.ErrorMessage");

        builder.ToTable("ErrorMessage");

        builder.Property(e => e.ErrorCode).HasMaxLength(50);

        builder.Property(e => e.Message).HasMaxLength(150);

        builder.Property(e => e.Options).HasMaxLength(50);

        builder.Property(e => e.Title).HasMaxLength(50);
    }
}