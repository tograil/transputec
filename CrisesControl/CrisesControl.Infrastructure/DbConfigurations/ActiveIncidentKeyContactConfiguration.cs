using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ActiveIncidentKeyContactConfiguration : IEntityTypeConfiguration<ActiveIncidentKeyContact>
{
    public void Configure(EntityTypeBuilder<ActiveIncidentKeyContact> builder)
    {
        builder.ToTable("ActiveIncidentKeyContact");

        builder.HasIndex(e => e.IncidentActivationId, "IDX_IncidentActivationId")
                .HasFillFactor(100);

        builder.HasIndex(e => e.UserId, "IX_UserId");
    }
}