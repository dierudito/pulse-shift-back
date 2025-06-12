using dm.PulseShift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dm.PulseShift.Infra.Data.Mappings;

public class DayOffMap : IEntityTypeConfiguration<DayOff>
{
    public void Configure(EntityTypeBuilder<DayOff> builder)
    {
        builder.ToTable("DaysOff");
        builder.HasKey(nwd => nwd.Id);

        builder.Property(nwd => nwd.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.HasIndex(nwd => nwd.Date)
            .IsUnique();

        builder.Property(nwd => nwd.Description)
            .HasMaxLength(500);

        builder.Property(nwd => nwd.CreatedAt)
            .IsRequired();

        builder.Property(nwd => nwd.UpdatedAt)
            .IsRequired(false);
    }
}
