using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserCommConfiguration : IEntityTypeConfiguration<UserComm>
{
    public void Configure(EntityTypeBuilder<UserComm> builder)
    {
        builder.HasKey(e => e.UserCommsId)
            .HasName("PK_dbo.UserComms");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => new {e.UserId, e.MethodId, e.MessageType}, "IDX_Method_Type_User");

        builder.Property(e => e.MessageType).HasMaxLength(20);

        builder.Property(e => e.Priority).HasDefaultValueSql("((1))");
        
        builder.ToTable("UserComms");
    }
}