using dm.PulseShift.Application;
using dm.PulseShift.Infra.CrossCutting.IoC.Configurations;
using dm.PulseShift.Infra.CrossCutting.Shared;
using System.Text.Json.Serialization;

namespace dm.PulseShift.bff.Extensions;

public static class ServiceCollectionExtension
{
    internal static IServiceCollection RegisterServices(this IServiceCollection services) =>
        services
        .AddDatabaseConfiguration()
        .AddCrossOrigin()
        .AddDocumentation()
        .ConfigureGlobalJsonOptions()
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
                .AllowAnyOrigin()
            //.AllowCredentials()
                )); 

    public static IServiceCollection ConfigureGlobalJsonOptions(this IServiceCollection services) =>
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.WriteIndented = true; // Cuidado com o impacto em produção
            // options.SerializerOptions.PropertyNameCaseInsensitive = true;
        });
}