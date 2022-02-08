using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> builder)
    {
        builder.HasNoKey();

        builder.ToView("User_Group");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.FirstName)
            .HasMaxLength(70)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.GroupId).HasColumnName("GroupID");

        builder.Property(e => e.GroupName).HasMaxLength(100);

        builder.Property(e => e.LastName)
            .HasMaxLength(70)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.UniqueId).HasColumnName("UniqueID");

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}