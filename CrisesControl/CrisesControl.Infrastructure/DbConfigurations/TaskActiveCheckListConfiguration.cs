using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskActiveCheckListConfiguration : IEntityTypeConfiguration<TaskActiveCheckList>
{
    public void Configure(EntityTypeBuilder<TaskActiveCheckList> builder)
    {
        builder.HasKey(e => e.ActiveCheckListId)
            .HasName("PK_dbo.TaskActiveCheckList");

        builder.ToTable("TaskActiveCheckList");

        builder.Property(e => e.ActiveCheckListId).HasColumnName("ActiveCheckListID");

        builder.Property(e => e.ActiveTaskId).HasColumnName("ActiveTaskID");

        builder.Property(e => e.CheckListId).HasColumnName("CheckListID");
    }
}