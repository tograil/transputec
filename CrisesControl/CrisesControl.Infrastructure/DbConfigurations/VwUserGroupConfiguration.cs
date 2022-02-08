using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class VwUserGroupConfiguration : IEntityTypeConfiguration<VwUserGroup>
{
    public void Configure(EntityTypeBuilder<VwUserGroup> builder)
    {
        builder.HasNoKey();

        builder.ToView("vw_UserGroups");

        builder.Property(e => e.RowId).ValueGeneratedOnAdd();
    }
}