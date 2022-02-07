using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class AdHocTaskParticipantConfiguration : IEntityTypeConfiguration<AdHocTaskParticipant>
{
    public void Configure(EntityTypeBuilder<AdHocTaskParticipant> builder)
    {
        builder.ToTable("AdHocTaskParticipant");

        builder.Property(e => e.Id).HasColumnName("ID");

        builder.Property(e => e.ActiveIncidentTaskId).HasColumnName("ActiveIncidentTaskID");

        builder.Property(e => e.ParticipantGroupId).HasColumnName("ParticipantGroupID");

        builder.Property(e => e.ParticipantTypeId).HasColumnName("ParticipantTypeID");

        builder.Property(e => e.ParticipantUserId).HasColumnName("ParticipantUserID");
    }
}