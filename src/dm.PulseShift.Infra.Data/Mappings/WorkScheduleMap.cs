using dm.PulseShift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dm.PulseShift.Infra.Data.Mappings;

public class WorkScheduleMap : IEntityTypeConfiguration<WorkSchedule>
{
    public void Configure(EntityTypeBuilder<WorkSchedule> builder)
    {
        builder.ToTable("WorkSchedules");
        builder.HasKey(ws => ws.Id);
        builder.Property(ws => ws.Id).ValueGeneratedOnAdd();
        builder.Property(ws => ws.WorkTime)
            .HasColumnType("time")
            .IsRequired(); 
        builder.Property(ws => ws.EntryType)
            .HasConversion<string>()
            .IsRequired();
        builder.HasIndex(ws => ws.EntryType)
            .IsUnique();

        builder.Property(ws => ws.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(ws => ws.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired(false);
    }
}