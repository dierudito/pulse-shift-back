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
            options.UseNpgsql(ApiConfigurations.ConnectionString);
#if (DEBUG)
            options.EnableSensitiveDataLogging();
#endif
        });
}