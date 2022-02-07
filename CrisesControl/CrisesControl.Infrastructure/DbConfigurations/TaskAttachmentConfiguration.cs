using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
{
    public void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        builder.HasKey(e => e.AttachmentId);

        builder.ToTable("TaskAttachment");

        builder.Property(e => e.AttachmentId).HasColumnName("AttachmentID");

        builder.Property(e => e.ActiveTaskId).HasColumnName("ActiveTaskID");

        builder.Property(e => e.FileName).HasMaxLength(200);

        builder.Property(e => e.SourceFileName).HasMaxLength(200);

        builder.Property(e => e.TaskActionId).HasColumnName("TaskActionID");
    }
}