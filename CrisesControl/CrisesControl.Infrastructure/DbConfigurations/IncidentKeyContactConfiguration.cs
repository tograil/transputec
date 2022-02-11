using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentKeyContactConfiguration : IEntityTypeConfiguration<IncidentKeyContact>
{
    public void Configure(EntityTypeBuilder<IncidentKeyContact> builder)
    {
        builder.ToTable("IncidentKeyContact");

        builder.HasIndex(e => e.IncidentId, "IDX_IncidentId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.UserId, "IX_UserId");
    }
}