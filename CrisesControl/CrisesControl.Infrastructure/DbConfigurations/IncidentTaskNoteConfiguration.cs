using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class IncidentTaskNoteConfiguration : IEntityTypeConfiguration<IncidentTaskNote>
{
    public void Configure(EntityTypeBuilder<IncidentTaskNote> builder)
    {
        builder.HasKey(e => e.IncidentTaskNotesId)
            .HasName("PK_dbo.IncidentTaskNotes");

        builder.Property(e => e.IncidentTaskNotesId).HasColumnName("IncidentTaskNotesID");

        builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

        builder.Property(e => e.NoteType).HasMaxLength(20);

        builder.Property(e => e.ObjectId).HasColumnName("ObjectID");

        builder.Property(e => e.UserId).HasColumnName("UserID");

        builder.ToTable("IncidentTaskNote");
    }
}