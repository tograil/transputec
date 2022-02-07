using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class ExTriggerAttachmentConfiguration : IEntityTypeConfiguration<ExTriggerAttachment>
{
    public void Configure(EntityTypeBuilder<ExTriggerAttachment> builder)
    {
        builder.ToTable("ExTriggerAttachment");
    }
}