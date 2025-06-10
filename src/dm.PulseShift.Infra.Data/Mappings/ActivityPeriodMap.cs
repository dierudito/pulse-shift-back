using dm.PulseShift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dm.PulseShift.Infra.Data.Mappings;

public class ActivityPeriodMap : IEntityTypeConfiguration<ActivityPeriod>
{
    public void Configure(EntityTypeBuilder<ActivityPeriod> builder)
    {
        builder.ToTable("ActivityPeriods");
        builder.HasKey(ap => ap.Id);
        builder.Property(ap => ap.StartDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(ap => ap.EndDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
        builder.Property(ap => ap.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();
        builder.Property(ap => ap.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.HasOne(ap => ap.Activity)
            .WithMany(a => a.ActivityPeriods)
            .HasForeignKey(ap => ap.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ap => ap.StartTimeEntry)
            .WithMany(t => t.StartActivityPeriods)
            .HasForeignKey(ap => ap.AssociatedStartTimeEntryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ap => ap.EndTimeEntry)
            .WithMany(t => t.EndActivityPeriods)
            .HasForeignKey(ap => ap.AssociatedEndTimeEntryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
