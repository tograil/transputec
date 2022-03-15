using CrisesControl.Core.Incidents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentKeyholderConfiguration : IEntityTypeConfiguration<IncidentKeyholder>
{
    public void Configure(EntityTypeBuilder<IncidentKeyholder> builder)
    {
        builder.ToTable("IncidentKeyholder");
    }
}