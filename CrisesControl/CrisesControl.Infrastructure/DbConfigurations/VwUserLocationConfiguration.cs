using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class VwUserLocationConfiguration : IEntityTypeConfiguration<VwUserLocation>
{
    public void Configure(EntityTypeBuilder<VwUserLocation> builder)
    {
        builder.HasNoKey();

        builder.ToView("vw_UserLocations");

        builder.Property(e => e.RowId).ValueGeneratedOnAdd();
    }
}