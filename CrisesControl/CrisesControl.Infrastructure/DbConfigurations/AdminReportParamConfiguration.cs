using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AdminReportParamConfiguration : IEntityTypeConfiguration<AdminReportParam>
{
    public void Configure(EntityTypeBuilder<AdminReportParam> builder)
    {
        builder.HasKey(e => e.ParamId);

        builder.ToTable("AdminReportParam");

        builder.Property(e => e.ParamId).HasColumnName("ParamID");

        builder.Property(e => e.DefaultValue).HasMaxLength(50);

        builder.Property(e => e.ParamName).HasMaxLength(50);

        builder.Property(e => e.ParamType)
                .HasMaxLength(20)
                .IsUnicode(false);

        builder.Property(e => e.ReportId).HasColumnName("ReportID");
    }
}