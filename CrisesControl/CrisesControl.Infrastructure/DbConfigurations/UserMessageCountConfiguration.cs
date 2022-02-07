using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserMessageCountConfiguration : IEntityTypeConfiguration<UserMessageCount>
{
    public void Configure(EntityTypeBuilder<UserMessageCount> builder)
    {
        builder.HasNoKey();

        builder.ToView("UserMessageCount");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.UserId).HasColumnName("UserID");
    }
}