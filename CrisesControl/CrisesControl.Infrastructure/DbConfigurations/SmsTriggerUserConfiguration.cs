using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SmsTriggerUserConfiguration : IEntityTypeConfiguration<SmsTriggerUser>
{
    public void Configure(EntityTypeBuilder<SmsTriggerUser> builder)
    {
        builder.ToTable("SMSTriggerUser");

        builder.HasIndex(e => new {e.CompanyId, e.UserId}, "IDX_CompanyID_UserID");

        builder.HasIndex(e => e.PhoneNumber, "IDX_PhoneNumber");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.PhoneNumber).HasMaxLength(20);

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}