using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TwilioPriceByIsdcodeConfiguration : IEntityTypeConfiguration<TwilioPriceByIsdcode>
{
    public void Configure(EntityTypeBuilder<TwilioPriceByIsdcode> builder)
    {
        builder.HasNoKey();

        builder.ToView("Twilio_Price_ByISDCode");

        builder.Property(e => e.BasePrice).HasColumnType("decimal(18, 4)");

        builder.Property(e => e.ChannelType)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(e => e.CurrentPrice).HasColumnType("decimal(18, 4)");

        builder.Property(e => e.Isdcode)
            .HasMaxLength(50)
            .HasColumnName("ISDCode");
    }
}