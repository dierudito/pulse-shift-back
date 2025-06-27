using dm.PulseShift.Domain.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace dm.PulseShift.Infra.Data.Repositories;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
        .AddScoped<ITimeEntryRepository, TimeEntryRepository>()
        .AddScoped<IWorkScheduleRepository, WorkScheduleRepository>()
        .AddScoped<IDayOffRepository, DayOffRepository>()
        .AddScoped<IActivityRepository, ActivityRepository>()
        .AddScoped<IActivityPeriodRepository, ActivityPeriodRepository>()
        .AddScoped<IInsightsRepository, InsightsRepository>()
        .AddScoped<ISprintReportRepository, SprintReportRepository>()
        ;
}