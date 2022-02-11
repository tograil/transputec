using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TaskActiveCheckListUserResponseConfiguration : IEntityTypeConfiguration<TaskActiveCheckListUserResponse>
{
    public void Configure(EntityTypeBuilder<TaskActiveCheckListUserResponse> builder)
    {
        builder.HasKey(e => e.UserResponseId)
            .HasName("PK_dbo.TaskActiveCheckListResponseOptions");

        builder.ToTable("TaskActiveCheckListUserResponse");

        builder.Property(e => e.UserResponseId).HasColumnName("UserResponseID");

        builder.Property(e => e.ActiveCheckListId).HasColumnName("ActiveCheckListID");

        builder.Property(e => e.ActiveReponseId).HasColumnName("ActiveReponseID");
    }
}