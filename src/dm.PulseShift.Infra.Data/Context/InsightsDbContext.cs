using dm.PulseShift.Domain.Entities.Insights;
using Microsoft.EntityFrameworkCore;

namespace dm.PulseShift.Infra.Data.Context;

public class InsightsDbContext(DbContextOptions<InsightsDbContext> options) : DbContext(options)
{
    public DbSet<ActivitySummary> ActivitySummaries { get; set; } = default!;
    public DbSet<DailyProductivitySummary> DailyProductivitySummaries { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActivitySummary>().HasNoKey();
        modelBuilder.Entity<DailyProductivitySummary>().HasNoKey();
    }
}