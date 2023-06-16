using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MessageMethod = CrisesControl.Core.Messages.MessageMethod;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MessageMethodConfiguration : IEntityTypeConfiguration<MessageMethod>
{
    public void Configure(EntityTypeBuilder<MessageMethod> builder)
    {
        builder.HasKey(e => e.MessageMethhodId);

        builder.Property(e => e.MessageMethhodId).HasColumnName("MessageMethhodID");

        builder.Property(e => e.ActiveIncidentId).HasColumnName("ActiveIncidentID");

        builder.Property(e => e.IncidentId)
            .HasColumnName("IncidentID")
            .HasDefaultValueSql("((0))");

        builder.Property(e => e.MessageId).HasColumnName("MessageID");

        builder.Property(e => e.MethodId).HasColumnName("MethodID");

        builder.ToTable("MessageMethods");
        builder.HasOne(x => x.CommsMethod)
           .WithMany().HasForeignKey(x => x.MethodId);
    }
}