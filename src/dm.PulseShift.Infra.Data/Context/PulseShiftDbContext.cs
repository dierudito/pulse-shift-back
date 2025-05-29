using dm.PulseShift.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace dm.PulseShift.Infra.Data.Context;
public class PulseShiftDbContext(DbContextOptions<PulseShiftDbContext> options)
    : DbContext(options)
{
    public DbSet<TimeEntry> TimeEntries { get; set; } = default!;
    public DbSet<WorkSchedule> WorkSchedules { get; set; } = default!;
    public DbSet<DayOff> DaysOff { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}