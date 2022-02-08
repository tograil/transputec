using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class MailMergeConfiguration : IEntityTypeConfiguration<MailMerge>
{
    public void Configure(EntityTypeBuilder<MailMerge> builder)
    {
        builder.HasNoKey();

        builder.ToTable("Mail_Merge");

        builder.Property(e => e.CompanyId)
            .HasMaxLength(50)
            .IsUnicode(false)
            .HasColumnName("CompanyID");

        builder.Property(e => e.CompanyName)
            .HasMaxLength(150)
            .IsUnicode(false);

        builder.Property(e => e.ContactName)
            .HasMaxLength(100)
            .IsUnicode(false);

        builder.Property(e => e.Email)
            .HasMaxLength(150)
            .IsUnicode(false)
            .HasColumnName("EMail");

        builder.Property(e => e.Email2)
            .HasMaxLength(150)
            .IsUnicode(false)
            .HasColumnName("EMail2");

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("ID");

        builder.Property(e => e.Phone)
            .HasMaxLength(100)
            .IsUnicode(false);
    }
}