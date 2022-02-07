using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PublicAlertUserConfiguration : IEntityTypeConfiguration<PublicAlertUser>
{
    public void Configure(EntityTypeBuilder<PublicAlertUser> builder)
    {
        builder.HasKey(e => e.UserListId);

        builder.ToTable("PublicAlertUser");

        builder.Property(e => e.UserListId).HasColumnName("UserListID");

        builder.Property(e => e.EmailId)
            .HasMaxLength(150)
            .HasColumnName("EmailID");

        builder.Property(e => e.ListId).HasColumnName("ListID");

        builder.Property(e => e.MobileNo).HasMaxLength(20);
    }
}