using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PasswordChangeHistoryConfiguration : IEntityTypeConfiguration<PasswordChangeHistory>
{
    public void Configure(EntityTypeBuilder<PasswordChangeHistory> builder)
    {
        builder.ToTable("PasswordChangeHistory");

        builder.Property(e => e.LastPassword).HasMaxLength(50);
    }
}