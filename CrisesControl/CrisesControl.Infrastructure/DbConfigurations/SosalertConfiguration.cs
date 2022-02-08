using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SosalertConfiguration : IEntityTypeConfiguration<Sosalert>
{
    public void Configure(EntityTypeBuilder<Sosalert> builder)
    {
        builder.ToTable("SOSAlert");

        builder.Property(e => e.SosalertId).HasColumnName("SOSAlertID");

        builder.Property(e => e.ActiveIncidentId).HasColumnName("ActiveIncidentID");

        builder.Property(e => e.AlertType)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.CallbackOption).HasDefaultValueSql("((0))");

        builder.Property(e => e.Latitude)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.Longitude)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.MessageListId).HasColumnName("MessageListID");

        builder.Property(e => e.ResponseId).HasColumnName("ResponseID");

        builder.Property(e => e.ResponseLabel).HasMaxLength(150);

        builder.Property(e => e.ResponseTimeGmt).HasColumnName("ResponseTimeGMT");

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}