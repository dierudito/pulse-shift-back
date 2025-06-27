using dm.PulseShift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dm.PulseShift.Infra.Data.Mappings;

public class SprintReportConfiguration : IEntityTypeConfiguration<SprintReport>
{
    public void Configure(EntityTypeBuilder<SprintReport> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.NomeSprint).IsRequired().HasMaxLength(100);
    }
}