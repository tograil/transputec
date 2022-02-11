using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyMessageResponseConfiguration : IEntityTypeConfiguration<CompanyMessageResponse>
{
    public void Configure(EntityTypeBuilder<CompanyMessageResponse> builder)
    {

        builder.HasKey(e => e.ResponseId)
            .HasName("PK_dbo.CompanyMessageResponse");

        builder.ToTable("CompanyMessageResponse");

        builder.Property(e => e.ResponseId).HasColumnName("ResponseID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.Description).HasMaxLength(500);

        builder.Property(e => e.LibResponseId).HasColumnName("LibResponseID");

        builder.Property(e => e.MessageType).HasMaxLength(10);

        builder.Property(e => e.ResponseLabel).HasMaxLength(50);

        builder.Property(e => e.SafetyAckAction).HasMaxLength(25);

        builder.Property(e => e.UpdatedOn).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

    }
}