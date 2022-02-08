using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AdminReportConfiguration : IEntityTypeConfiguration<AdminReport>
{
    public void Configure(EntityTypeBuilder<AdminReport> builder)
    {
        builder.HasKey(e => e.ReportId);

        builder.ToTable("AdminReport");

        builder.Property(e => e.ReportId).HasColumnName("ReportID");

        builder.Property(e => e.Description).HasMaxLength(250);

        builder.Property(e => e.DownloadFileName).HasMaxLength(150);

        builder.Property(e => e.ReportName).HasMaxLength(150);

        builder.Property(e => e.SourceType).HasMaxLength(25);
    }
}