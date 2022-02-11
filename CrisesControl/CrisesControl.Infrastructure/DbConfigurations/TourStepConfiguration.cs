using CrisesControl.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrisesControl.Infrastructure.DbConfigurations;

public class TourStepConfiguration : IEntityTypeConfiguration<TourStep>
{
    public void Configure(EntityTypeBuilder<TourStep> builder)
    {
        builder.ToTable("TourStep");

        builder.Property(e => e.TourStepId).HasColumnName("TourStepID");

        builder.Property(e => e.ActionType)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.HighlightKey)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.Margin)
            .HasMaxLength(5)
            .IsUnicode(false);

        builder.Property(e => e.ModalType)
            .HasMaxLength(15)
            .IsUnicode(false);

        builder.Property(e => e.NextAction)
            .HasMaxLength(250)
            .IsUnicode(false);

        builder.Property(e => e.NextLabel).HasMaxLength(50);

        builder.Property(e => e.OnEnterEvent)
            .HasMaxLength(250)
            .IsUnicode(false);

        builder.Property(e => e.OnLeaveEvent)
            .HasMaxLength(250)
            .IsUnicode(false);

        builder.Property(e => e.Overlay)
            .IsRequired()
            .HasDefaultValueSql("((1))");

        builder.Property(e => e.PrevAction)
            .HasMaxLength(250)
            .IsUnicode(false);

        builder.Property(e => e.PrevLabel).HasMaxLength(50);

        builder.Property(e => e.Status).HasDefaultValueSql("((1))");

        builder.Property(e => e.StepDesc).HasMaxLength(1000);

        builder.Property(e => e.StepKey)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.StepTitle).HasMaxLength(150);

        builder.Property(e => e.TipPosition)
            .HasMaxLength(10)
            .IsUnicode(false);

        builder.Property(e => e.TourKey)
            .HasMaxLength(50)
            .IsUnicode(false);

        builder.Property(e => e.TourName)
            .HasMaxLength(100)
            .IsUnicode(false);
    }
}