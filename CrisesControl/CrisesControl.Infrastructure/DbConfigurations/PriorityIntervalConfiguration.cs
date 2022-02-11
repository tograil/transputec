using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PriorityIntervalConfiguration : IEntityTypeConfiguration<PriorityInterval>
{
    public void Configure(EntityTypeBuilder<PriorityInterval> builder)
    {
        builder.ToTable("PriorityInterval");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyID");

        builder.HasIndex(e => e.MessageType, "IDX_MessageType");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.CascadingPlanId).HasColumnName("CascadingPlanID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.MessageType)
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(e => e.Methods)
            .HasMaxLength(20)
            .IsUnicode(false);
    }
}