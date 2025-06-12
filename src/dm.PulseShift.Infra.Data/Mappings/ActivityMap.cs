using dm.PulseShift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dm.PulseShift.Infra.Data.Mappings;

public class ActivityMap : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("Activities");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.CardCode)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(a => a.CardLink)
            .HasMaxLength(500);
        builder.Property(a => a.Description)
            .HasMaxLength(500);
        builder.Property(a => a.CreatedAt)
            .IsRequired();
        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);
        builder.HasIndex(a => a.CardCode)
            .IsUnique()
            .HasDatabaseName("IX_Activities_CardCode");
        builder.Ignore(a => a.FirstOverallStartDate);
        builder.Ignore(a => a.LastOverallEndDate);
    }
}
