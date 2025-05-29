using dm.PulseShift.Application;
using dm.PulseShift.Infra.CrossCutting.IoC.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace dm.PulseShift.Automation.Extensions;

public static class ServiceCollectionExtension
{
    internal static IServiceCollection RegisterServices(this IServiceCollection services) =>
        services
        .AddDatabaseConfiguration()
        .ResolveDependencies()
        .AddApplication();
}