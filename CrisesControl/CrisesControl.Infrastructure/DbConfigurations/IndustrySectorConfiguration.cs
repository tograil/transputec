using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IndustrySectorConfiguration : IEntityTypeConfiguration<IndustrySector>
{
    public void Configure(EntityTypeBuilder<IndustrySector> builder)
    {
        builder.HasKey(e => e.SectorId);

        builder.ToTable("IndustrySector");

        builder.Property(e => e.SectorId).HasColumnName("SectorID");

        builder.Property(e => e.CreatedDate).HasColumnType("datetime");

        builder.Property(e => e.SectorName).HasMaxLength(100);

        builder.Property(e => e.UpdatedDate).HasColumnType("datetime");
    }
}