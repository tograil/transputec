using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ActiveMessageResponseConfiguration : IEntityTypeConfiguration<ActiveMessageResponse>
{
    public void Configure(EntityTypeBuilder<ActiveMessageResponse> builder)
    {
        builder.HasKey(e => e.MessageResponseId)
                .HasName("PK_dbo.ActiveMessageResponse");

        builder.ToTable("ActiveMessageResponse");

        builder.Property(e => e.MessageResponseId).HasColumnName("MessageResponseID");

        builder.Property(e => e.ActiveIncidentId)
                .HasColumnName("ActiveIncidentID")
                .HasDefaultValueSql("((0))");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.ResponseId).HasColumnName("ResponseID");

        builder.Property(e => e.ResponseLabel).HasMaxLength(50);

        builder.Property(e => e.SafetyAckAction).HasMaxLength(25);
    }
}