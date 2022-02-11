using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasIndex(e => e.CompanyId, "IDX_CompanyId")
            .HasFillFactor(100);

        builder.HasIndex(e => new {e.FirstName, e.LastName}, "IDX_FirstName_LastName");

        builder.HasIndex(e => new {e.PrimaryEmail, e.Password}, "IDX_PrimaryEmail")
            .HasFillFactor(100);

        builder.HasIndex(e => e.RegisteredUser, "IDX_RegisteredUser")
            .HasFillFactor(100);

        builder.HasIndex(e => e.UniqueGuiId, "IDX_UniqueGUID")
            .HasFillFactor(100);

        builder.HasIndex(e => e.UserRole, "IDX_UserRole");

        builder.Property(e => e.ExpirePassword)
            .IsRequired()
            .HasDefaultValueSql("((1))");

        builder.Property(e => e.FirstName)
            .HasMaxLength(70)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Isdcode)
            .HasMaxLength(10)
            .HasColumnName("ISDCode")
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Landline)
            .HasMaxLength(20)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.LastLocationUpdate).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.LastName)
            .HasMaxLength(70)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Lat).HasMaxLength(20);

        builder.Property(e => e.Llisdcode)
            .HasMaxLength(10)
            .HasColumnName("LLISDCode")
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Lng).HasMaxLength(20);

        builder.Property(e => e.MobileNo)
            .HasMaxLength(20)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Otpcode)
            .HasMaxLength(10)
            .HasColumnName("OTPCode");

        builder.Property(e => e.Otpexpiry)
            .HasColumnName("OTPExpiry")
            .HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.Password)
            .HasMaxLength(50)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.PasswordChangeDate).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.PrimaryEmail)
            .HasMaxLength(150)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.SecondaryEmail)
            .HasMaxLength(150)
            .UseCollation("Latin1_General_BIN2");

        builder.Property(e => e.Smstrigger).HasColumnName("SMSTrigger");

        builder.Property(e => e.TrackingEndTime).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.TrackingStartTime).HasDefaultValueSql("('1900-01-01T00:00:00.000')");

        builder.Property(e => e.UniqueGuiId)
            .HasMaxLength(70)
            .HasColumnName("UniqueGuiID");

        builder.Property(e => e.UserHash)
            .HasMaxLength(32)
            .IsUnicode(false)
            .IsFixedLength();

        builder.Property(e => e.UserLanguage).HasMaxLength(20);

        builder.Property(e => e.UserPhoto).HasMaxLength(70);

        builder.Property(e => e.UserRole).HasMaxLength(10);
    }
}