using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibIncidentTypeConfiguration : IEntityTypeConfiguration<LibIncidentType>
{
    public void Configure(EntityTypeBuilder<LibIncidentType> builder)
    {
        builder.ToTable("LibIncidentType");

        builder.Property(e => e.Name).HasMaxLength(50);
    }
}