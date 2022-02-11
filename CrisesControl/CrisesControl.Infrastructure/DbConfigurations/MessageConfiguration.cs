using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Message");

        builder.HasIndex(e => e.MessageType, "IDX_MessageType");

        builder.HasIndex(e => e.CreatedOn, "IDX_Message_CreatedOn");

        builder.HasIndex(e => new {e.MessageType, e.CompanyId}, "IDX_Message_TYpe_CompanyID");

        builder.HasIndex(e => e.CompanyId, "IX_CompanyId");

        builder.HasIndex(e => e.IncidentActivationId, "IX_IncidentActivationID");

        builder.HasIndex(e => e.Priority, "IX_Priority");

        builder.HasIndex(e => e.Status, "IX_Status");

        builder.Property(e => e.ActiveIncidentTaskId).HasColumnName("ActiveIncidentTaskID");

        builder.Property(e => e.CascadePlanId).HasColumnName("CascadePlanID");

        builder.Property(e => e.CreatedTimeZone).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.MessageActionType).HasDefaultValueSql("((0))");

        builder.Property(e => e.MessageSourceAction).HasMaxLength(50);

        builder.Property(e => e.MessageText).UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.MessageType).HasMaxLength(50);

        builder.Property(e => e.ParentId).HasColumnName("ParentID");
    }
}