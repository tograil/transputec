using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AddressLinkConfiguration : IEntityTypeConfiguration<AddressLink>
{
    public void Configure(EntityTypeBuilder<AddressLink> builder)
    {
        builder.ToTable("AddressLink");
        builder.HasOne(x => x.Address)
          .WithMany().HasForeignKey(x => x.AddressLinkId);
    }
}