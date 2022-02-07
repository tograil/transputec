using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AcademyAccessConfiguration : IEntityTypeConfiguration<AcademyAccess>
{
    public void Configure(EntityTypeBuilder<AcademyAccess> builder)
    {
        builder.HasNoKey();

        builder.ToTable("AcademyAccess");

        builder.Property(e => e.AccessId)
                .ValueGeneratedOnAdd()
                .HasColumnName("AccessID");

        builder.Property(e => e.VideoId).HasColumnName("VideoID");
    }
}