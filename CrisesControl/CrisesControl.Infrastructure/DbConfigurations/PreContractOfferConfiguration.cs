using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class PreContractOfferConfiguration : IEntityTypeConfiguration<PreContractOffer>
{
    public void Configure(EntityTypeBuilder<PreContractOffer> builder)
    {
        builder.HasKey(e => e.OfferId)
            .HasName("PK_dbo.PreContractOffer");

        builder.ToTable("PreContractOffer");

        builder.Property(e => e.OfferId).HasColumnName("OfferID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.KeyHolderRate).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.MonthlyContractValue).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.StaffRate).HasColumnType("decimal(18, 2)");

        builder.Property(e => e.YearlyContractValue).HasColumnType("decimal(18, 2)");
    }
}