using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class SconnectionConfiguration : IEntityTypeConfiguration<Sconnection>
{
    public void Configure(EntityTypeBuilder<Sconnection> builder)
    {
        builder.HasKey(e => e.UserConnectionId)
            .HasName("PK_dbo.SConnection");

        builder.ToTable("SConnection");

        builder.HasIndex(e => e.UsersUserId, "IX_Users_UserId");

        builder.Property(e => e.UsersUserId).HasColumnName("Users_UserId");
    }
}