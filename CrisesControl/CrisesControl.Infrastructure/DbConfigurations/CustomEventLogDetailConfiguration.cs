using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CustomEventLogDetailConfiguration : IEntityTypeConfiguration<CustomEventLogDetail>
{
    public void Configure(EntityTypeBuilder<CustomEventLogDetail> builder)
    {
        builder.HasKey(e => e.EventLogId);

        builder.ToTable("CustomEventLogDetail");

        builder.Property(e => e.EventLogId).HasColumnName("EventLogID");

        builder.Property(e => e.Cmtaction).HasColumnName("CMTAction");

        builder.Property(e => e.EventLogHeaderId).HasColumnName("EventLogHeaderID");
    }
}