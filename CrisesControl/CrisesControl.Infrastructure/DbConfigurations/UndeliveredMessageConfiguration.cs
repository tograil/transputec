using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UndeliveredMessageConfiguration : IEntityTypeConfiguration<UndeliveredMessage>
{
    public void Configure(EntityTypeBuilder<UndeliveredMessage> builder)
    {
        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .HasDefaultValueSql("(newsequentialid())");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.MessageDeviceId).HasColumnName("MessageDeviceID");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.MethodName)
            .HasMaxLength(10)
            .IsUnicode(false);
    }
}