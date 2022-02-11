using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SosactionConfiguration : IEntityTypeConfiguration<Sosaction>
{
    public void Configure(EntityTypeBuilder<Sosaction> builder)
    {
        builder.ToTable("SOSActions");

        builder.Property(e => e.SosactionId).HasColumnName("SOSActionID");

        builder.Property(e => e.ActionType).HasMaxLength(20);

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.SosalertId).HasColumnName("SOSAlertID");
    }
}