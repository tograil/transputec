using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AcademyVideoConfiguration : IEntityTypeConfiguration<AcademyVideo>
{
    public void Configure(EntityTypeBuilder<AcademyVideo> builder)
    {

        builder.HasKey(e => e.VideoId);

        builder.ToTable("AcademyVideo");

        builder.Property(e => e.VideoId).HasColumnName("VideoID");

        builder.Property(e => e.SourceType)
                .HasMaxLength(20)
                .IsUnicode(false);

        builder.Property(e => e.SourceUrl)
                .HasMaxLength(500)
                .HasColumnName("SourceURL");

        builder.Property(e => e.Status).HasDefaultValueSql("((1))");

        builder.Property(e => e.VideoDescription).HasMaxLength(1000);

        builder.Property(e => e.VideoImage).HasMaxLength(500);

        builder.Property(e => e.VideoKey)
                .HasMaxLength(100)
                .IsUnicode(false);

        builder.Property(e => e.VideoTitle).HasMaxLength(250);
        
    }
}