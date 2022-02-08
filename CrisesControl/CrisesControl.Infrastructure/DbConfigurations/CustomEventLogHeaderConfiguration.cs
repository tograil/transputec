using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CustomEventLogHeaderConfiguration : IEntityTypeConfiguration<CustomEventLogHeader>
{
    public void Configure(EntityTypeBuilder<CustomEventLogHeader> builder)
    {
        builder.HasKey(e => e.EventLogHeaderId);

        builder.ToTable("CustomEventLogHeader");

        builder.Property(e => e.EventLogHeaderId).HasColumnName("EventLogHeaderID");

        builder.Property(e => e.ActiveIncidentId).HasColumnName("ActiveIncidentID");
    }
}