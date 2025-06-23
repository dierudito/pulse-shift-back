using dm.PulseShift.Infra.CrossCutting.Shared;
using dm.PulseShift.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace dm.PulseShift.Infra.CrossCutting.IoC.Configurations;
public static class DatabaseConfig
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services) =>
        services
        .AddEntityFrameworkConfiguration();

    private static IServiceCollection AddEntityFrameworkConfiguration(this IServiceCollection services) =>
    services
        .AddDbContext<PulseShiftDbContext>(options =>
        {
            options.UseSqlServer(ApiConfigurations.ConnectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                });
#if (DEBUG)
            options.EnableSensitiveDataLogging();
#endif
        })
        .AddDbContext<InsightsDbContext>(options => options.UseSqlite(ApiConfigurations.InsightsDbConnectionString));
}