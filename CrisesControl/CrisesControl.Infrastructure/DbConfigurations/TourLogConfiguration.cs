using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TourLogConfiguration : IEntityTypeConfiguration<TourLog>
{
    public void Configure(EntityTypeBuilder<TourLog> builder)
    {
        builder.ToTable("TourLog");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.TourName)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.TourStepId).HasColumnName("TourStepID");

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}