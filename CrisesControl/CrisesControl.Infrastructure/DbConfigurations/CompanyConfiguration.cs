using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Company");

        builder.Property(e => e.AndroidLogo).HasMaxLength(100);

        builder.Property(e => e.AnniversaryDate).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.CompanyLogoPath).HasMaxLength(100);

        builder.Property(e => e.CompanyName)
                .HasMaxLength(200)
                .HasColumnName("Company_Name");

        builder.Property(e => e.CompanyProfile).HasMaxLength(150);

        builder.Property(e => e.ContactLogoPath).HasMaxLength(100);

        builder.Property(e => e.CustomerId).HasMaxLength(100);

        builder.Property(e => e.Fax).HasMaxLength(20);

        builder.Property(e => e.IOslogo)
                .HasMaxLength(100)
                .HasColumnName("iOSLogo");

        builder.Property(e => e.InvitationCode)
                .HasMaxLength(20)
                .IsUnicode(false);

        builder.Property(e => e.Isdcode)
                .HasMaxLength(10)
                .HasColumnName("ISDCode");

        builder.Property(e => e.PlanDrdoc)
                .HasMaxLength(100)
                .HasColumnName("PlanDRDoc");

        builder.Property(e => e.Sector).HasMaxLength(150);

        builder.Property(e => e.SwitchBoardPhone).HasMaxLength(20);

        builder.Property(e => e.UniqueKey)
            .HasMaxLength(150)
                .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Website).HasMaxLength(250);

        builder.Property(e => e.WindowsLogo).HasMaxLength(100);
    }
}