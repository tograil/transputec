using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UsersSearchConfiguration : IEntityTypeConfiguration<UsersSearch>
{
    public void Configure(EntityTypeBuilder<UsersSearch> builder)
    {
        builder.HasNoKey();

        builder.ToTable("UsersSearch");

        builder.Property(e => e.SearchFieldEncrypted).HasColumnName("SearchField_Encrypted");
    }
}