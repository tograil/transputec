using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentMessageResponseConfiguration : IEntityTypeConfiguration<IncidentMessageResponse>
{
    public void Configure(EntityTypeBuilder<IncidentMessageResponse> builder)
    {
        builder.HasKey(e => e.IncidentResponseId);

        builder.ToTable("IncidentMessageResponse");

        builder.Property(e => e.IncidentResponseId).HasColumnName("IncidentResponseID");

        builder.Property(e => e.IncidentId).HasColumnName("IncidentID");

        builder.Property(e => e.ResponseId).HasColumnName("ResponseID");
    }
}