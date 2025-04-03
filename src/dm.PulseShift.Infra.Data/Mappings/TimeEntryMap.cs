using dm.PulseShift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dm.PulseShift.Infra.Data.Mappings;
public class TimeEntryMap : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");
        builder.HasKey(te => te.Id);
        builder.Property(te => te.Id).ValueGeneratedOnAdd();
        builder.Property(te => te.Description).HasMaxLength(500);

        builder.Property(te => te.EntryDate)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(te => te.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(te => te.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);

        builder.Property(te => te.EntryType)
            .HasConversion<string>()
            .IsRequired();
    }
}