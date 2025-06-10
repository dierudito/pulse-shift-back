using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Infra.CrossCutting.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace dm.PulseShift.Infra.Data.Context;
public class PulseShiftDbContext(DbContextOptions<PulseShiftDbContext> options)
    : DbContext(options)
{
    public static readonly TimeZoneInfo SaoPauloTimeZone = TimeZoneHelper.GetSaoPauloTimeZone();

    public DbSet<TimeEntry> TimeEntries { get; set; } = default!;
    public DbSet<WorkSchedule> WorkSchedules { get; set; } = default!;
    public DbSet<DayOff> DaysOff { get; set; } = default!;
    public DbSet<Activity> Activities { get; set; } = default!;
    public DbSet<ActivityPeriod> ActivityPeriods { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var saoPauloDateTimeConverter = new SaoPauloDateTimeConverter();
        var nullableSaoPauloDateTimeConverter = new NullableSaoPauloDateTimeConverter();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    //property.SetValueConverter(saoPauloDateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    //property.SetValueConverter(nullableSaoPauloDateTimeConverter);
                }
            }
        }
    }
}
