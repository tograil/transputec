using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PriorityMethodConfiguration : IEntityTypeConfiguration<PriorityMethod>
{
    public void Configure(EntityTypeBuilder<PriorityMethod> builder)
    {
        builder.ToTable("PriorityMethod");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.MessageType)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.Methods)
            .HasMaxLength(20)
            .IsUnicode(false);
    }
}