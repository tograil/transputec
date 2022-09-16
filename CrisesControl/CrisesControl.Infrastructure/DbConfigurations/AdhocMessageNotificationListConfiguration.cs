using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AdhocMessageNotificationListConfiguration : IEntityTypeConfiguration<AdhocMessageNotificationList>
{
    public void Configure(EntityTypeBuilder<AdhocMessageNotificationList> builder)
    {
        builder.ToTable("AdhocMessageNotificationList");

        builder.HasIndex(e => e.MessageId, "IDX_MessageId")
                .HasFillFactor(100);

        builder.HasIndex(e => e.ObjectMappingId, "IDX_ObjectMappingId")
                .HasFillFactor(100);

        builder.HasIndex(e => e.SourceObjectPrimaryId, "IDX_SourceObjectPrimaryId")
                .HasFillFactor(100);
        builder.HasOne(e => e.Department).WithMany().HasForeignKey(e => e.SourceObjectPrimaryId);
    }
}