using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskCheckListConfiguration : IEntityTypeConfiguration<TaskCheckList>
{
    public void Configure(EntityTypeBuilder<TaskCheckList> builder)
    {
        builder.HasKey(e => e.CheckListId)
            .HasName("PK_dbo.TaskCheckList");

        builder.ToTable("TaskCheckList");

        builder.Property(e => e.CheckListId).HasColumnName("CheckListID");

        builder.Property(e => e.TaskId).HasColumnName("TaskID");
    }
}