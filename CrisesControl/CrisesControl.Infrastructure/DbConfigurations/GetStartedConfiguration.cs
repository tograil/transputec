using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class GetStartedConfiguration : IEntityTypeConfiguration<GetStarted>
{
    public void Configure(EntityTypeBuilder<GetStarted> builder)
    {
        builder.HasKey(e => e.Gsid)
            .HasName("PK_dbo.GetStarted");

        builder.ToTable("GetStarted");

        builder.Property(e => e.Gsid).HasColumnName("GSId");

        builder.Property(e => e.Address1)
            .HasMaxLength(250)
            .HasColumnName("address1");

        builder.Property(e => e.Address2)
            .HasMaxLength(250)
            .HasColumnName("address2");

        builder.Property(e => e.Assdone).HasColumnName("assdone");

        builder.Property(e => e.Cisdcode)
            .HasMaxLength(10)
            .HasColumnName("CISDCode");

        builder.Property(e => e.City).HasMaxLength(100);

        builder.Property(e => e.CompanyName)
            .HasMaxLength(250)
            .HasColumnName("companyName");

        builder.Property(e => e.Country).HasMaxLength(5);

        builder.Property(e => e.DepartmentName).HasMaxLength(150);

        builder.Property(e => e.Depdone).HasColumnName("depdone");

        builder.Property(e => e.Email).HasMaxLength(150);

        builder.Property(e => e.FirstName).HasMaxLength(150);

        builder.Property(e => e.Incidone).HasColumnName("incidone");

        builder.Property(e => e.Landline).HasMaxLength(15);

        builder.Property(e => e.LastName).HasMaxLength(150);

        builder.Property(e => e.Llisdcode)
            .HasMaxLength(10)
            .HasColumnName("LLISDCode");

        builder.Property(e => e.Locdone).HasColumnName("locdone");

        builder.Property(e => e.Mobile).HasMaxLength(15);

        builder.Property(e => e.Password).HasMaxLength(50);

        builder.Property(e => e.Postcode).HasMaxLength(20);

        builder.Property(e => e.SessionId)
            .HasMaxLength(100)
            .HasColumnName("sessionId");

        builder.Property(e => e.State).HasMaxLength(100);

        builder.Property(e => e.SwtchPhone).HasMaxLength(15);

        builder.Property(e => e.Uisdcode)
            .HasMaxLength(10)
            .HasColumnName("UISDCode");

        builder.Property(e => e.Userdone).HasColumnName("userdone");

        builder.Property(e => e.Website).HasMaxLength(250);
    }
}