using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class QrtzCalendarConfiguration : IEntityTypeConfiguration<QrtzCalendar>
{
    public void Configure(EntityTypeBuilder<QrtzCalendar> builder)
    {
        builder.HasKey(e => new {e.SchedName, e.CalendarName});

        builder.ToTable("QRTZ_CALENDARS");

        builder.Property(e => e.SchedName)
            .HasMaxLength(100)
            .HasColumnName("SCHED_NAME");

        builder.Property(e => e.CalendarName)
            .HasMaxLength(200)
            .HasColumnName("CALENDAR_NAME");

        builder.Property(e => e.Calendar)
            .HasColumnType("image")
            .HasColumnName("CALENDAR");
    }
}