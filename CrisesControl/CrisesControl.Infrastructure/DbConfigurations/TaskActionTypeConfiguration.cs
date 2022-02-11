using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskActionTypeConfiguration : IEntityTypeConfiguration<TaskActionType>
{
    public void Configure(EntityTypeBuilder<TaskActionType> builder)
    {
        builder.ToTable("TaskActionType");

        builder.Property(e => e.TaskActionTypeId).HasColumnName("TaskActionTypeID");

        builder.Property(e => e.ActionTypeDescription).HasMaxLength(150);

        builder.Property(e => e.ActionTypeName).HasMaxLength(50);
    }
}