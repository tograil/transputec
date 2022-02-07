using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SecurityMethodConfiguration : IEntityTypeConfiguration<SecurityMethod>
{
    public void Configure(EntityTypeBuilder<SecurityMethod> builder)
    {
        builder.HasKey(e => e.MethodId);

        builder.ToTable("SecurityMethod");

        builder.Property(e => e.MethodId).HasColumnName("MethodID");

        builder.Property(e => e.MethodName)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.SecurityKey)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Target)
            .HasMaxLength(20)
            .IsUnicode(false);
    }
}