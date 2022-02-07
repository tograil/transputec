using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskCheckListResponseConfiguration : IEntityTypeConfiguration<TaskCheckListResponse>
{
    public void Configure(EntityTypeBuilder<TaskCheckListResponse> builder)
    {
        builder.HasKey(e => e.CheckListResponseId)
                .HasName("PK_dbo.TaskCheckListResponse");

        builder.ToTable("TaskCheckListResponse");

        builder.Property(e => e.CheckListResponseId).HasColumnName("CheckListResponseID");

        builder.Property(e => e.CheckListId).HasColumnName("CheckListID");

        builder.Property(e => e.ResponseId).HasColumnName("ResponseID");
    }
}