using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskActiveCheckListResponseConfiguration : IEntityTypeConfiguration<TaskActiveCheckListResponse>
{
    public void Configure(EntityTypeBuilder<TaskActiveCheckListResponse> builder)
    {
        builder.HasKey(e => e.ActiveCheckListResponseId);

        builder.ToTable("TaskActiveCheckListResponse");

        builder.Property(e => e.ActiveCheckListResponseId).HasColumnName("ActiveCheckListResponseID");

        builder.Property(e => e.ActiveCheckListId).HasColumnName("ActiveCheckListID");

        builder.Property(e => e.Response).HasMaxLength(500);

        builder.Property(e => e.ResponseId).HasColumnName("ResponseID");
    }
}