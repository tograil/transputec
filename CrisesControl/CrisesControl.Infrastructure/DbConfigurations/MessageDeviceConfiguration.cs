using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MessageDeviceConfiguration : IEntityTypeConfiguration<MessageDevice>
{
    public void Configure(EntityTypeBuilder<MessageDevice> builder)
    {
        builder.ToTable("MessageDevice");

        builder.HasIndex(e => e.Attempt, "IDX_Attempt")
            .HasFillFactor(100);

        builder.HasIndex(e => e.CloudMessageId, "IDX_CloudMessageId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.LockStatus, "IDX_LockStatus")
            .HasFillFactor(100);

        builder.HasIndex(e => e.MessageId, "IDX_MessageId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.MessageListId, "IDX_MessageListId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.Priority, "IDX_Priority")
            .HasFillFactor(100);

        builder.HasIndex(e => e.Status, "IDX_Status")
            .HasFillFactor(100);

        builder.HasIndex(e => e.UserDeviceId, "IDX_UserDeviceId")
            .HasFillFactor(100);

        builder.HasIndex(e => e.CompanyId, "IX_CompanyId");

        builder.Property(e => e.CloudMessageId).HasMaxLength(100);

        builder.Property(e => e.DeviceAddress).HasMaxLength(250);

        builder.Property(e => e.DeviceType).HasMaxLength(50);

        builder.Property(e => e.LockStatus).HasMaxLength(50);

        builder.Property(e => e.MessageText).UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Method).HasMaxLength(50);

        builder.Property(e => e.MobileIsd)
            .HasMaxLength(10)
            .HasColumnName("MobileISD")
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.MobileNo)
            .HasMaxLength(20)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.SirenOn).HasColumnName("SirenON");

        builder.Property(e => e.SoundFile)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.Status).HasMaxLength(50);

        builder.Property(e => e.UserEmail)
            .HasMaxLength(150)
            .UseCollation("Latin1_General_BIN2");
        builder.HasOne(x => x.MessageList)
         .WithMany().HasForeignKey(x => x.MessageListId);
    }
}