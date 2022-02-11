using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class LibIncidentConfiguration : IEntityTypeConfiguration<LibIncident>
{
    public void Configure(EntityTypeBuilder<LibIncident> builder)
    {
        builder.ToTable("LibIncident");

        builder.Property(e => e.IsSos).HasColumnName("IsSOS");

        builder.Property(e => e.LibIncodentIcon).HasMaxLength(100);

        builder.Property(e => e.Name).HasMaxLength(50);
    }
}