using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibIncidentActionConfiguration : IEntityTypeConfiguration<LibIncidentAction>
{
    public void Configure(EntityTypeBuilder<LibIncidentAction> builder)
    {
        builder.ToTable("LibIncidentAction");
    }
}