using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TextMessageLinkConfiguration : IEntityTypeConfiguration<TextMessageLink>
{
    public void Configure(EntityTypeBuilder<TextMessageLink> builder)
    {
        builder.HasKey(e => e.MessageLinkId)
            .HasName("PK_dbo.TextMessageLink");

        builder.ToTable("TextMessageLink");
    }
}