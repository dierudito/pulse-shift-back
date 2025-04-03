using dm.PulseShift.Application;
using dm.PulseShift.Infra.CrossCutting.IoC.Configurations;
using dm.PulseShift.Infra.CrossCutting.Shared;

namespace dm.PulseShift.bff.Extensions;

public static class ServiceCollectionExtension
{
    internal static IServiceCollection RegisterServices(this IServiceCollection services) =>
        services
        .AddDatabaseConfiguration()
        .AddCrossOrigin()
        .AddDocumentation()
        .ResolveDependencies()
        .AddApplication();

    public static IServiceCollection AddDocumentation(this IServiceCollection services) =>
        services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen(x =>
        {
            x.CustomSchemaIds(n => n.FullName);
        });

    public static IServiceCollection AddCrossOrigin(this IServiceCollection services) =>
        services.AddCors(
            options => options.AddPolicy(
                ApiConfigurations.CorsPolicyName,
            policy => policy
                .WithOrigins([
                    ApiConfigurations.BackendUrl,
                    ApiConfigurations.FrontendUrl
                    ])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                ));
}