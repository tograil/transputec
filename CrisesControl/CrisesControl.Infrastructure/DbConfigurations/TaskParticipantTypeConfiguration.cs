using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskParticipantTypeConfiguration : IEntityTypeConfiguration<TaskParticipantType>
{
    public void Configure(EntityTypeBuilder<TaskParticipantType> builder)
    {
        builder.ToTable("TaskParticipantType");

        builder.Property(e => e.TaskParticipantTypeId).HasColumnName("TaskParticipantTypeID");

        builder.Property(e => e.TaskParticipantTypeName).HasMaxLength(100);
    }
}