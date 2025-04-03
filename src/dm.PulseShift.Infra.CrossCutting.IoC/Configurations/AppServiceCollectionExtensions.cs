using dm.PulseShift.Domain;
using dm.PulseShift.Infra.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace dm.PulseShift.Infra.CrossCutting.IoC.Configurations;
public static class AppServiceCollectionExtensions
{
    public static IServiceCollection ResolveDependencies(this IServiceCollection services) =>
        services
            .AddDatabaseConfiguration()
            .AddAutoMapper()
            .AddDomainServices()
            .AddRepositories();
}
