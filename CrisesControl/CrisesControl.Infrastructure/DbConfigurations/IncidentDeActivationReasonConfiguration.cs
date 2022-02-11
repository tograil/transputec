using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentDeActivationReasonConfiguration : IEntityTypeConfiguration<IncidentDeActivationReason>
{
    public void Configure(EntityTypeBuilder<IncidentDeActivationReason> builder)
    {
        builder.ToTable("IncidentDeActivationReason");

            builder.HasIndex(e => e.IncidentActivationId, "IX_IncidentActivationId");

            builder.Property(e => e.Type).HasMaxLength(50);
    }
}