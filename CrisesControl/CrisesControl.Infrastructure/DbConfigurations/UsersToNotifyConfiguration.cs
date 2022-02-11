using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UsersToNotifyConfiguration : IEntityTypeConfiguration<UsersToNotify>
{
    public void Configure(EntityTypeBuilder<UsersToNotify> builder)
    {
        builder.ToTable("UsersToNotify");

        builder.HasIndex(e => e.MessageId, "IDX_MessageID");

        builder.HasIndex(e => e.UserId, "IDX_UserID");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.ActiveIncidentId).HasColumnName("ActiveIncidentID");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}