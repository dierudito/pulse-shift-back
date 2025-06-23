using dm.PulseShift.Application;
using dm.PulseShift.Infra.CrossCutting.IoC.Configurations;
using dm.PulseShift.Infra.CrossCutting.Shared;
using Microsoft.OpenApi.Models;
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
        .AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Workforce Journey Management API",
                Description = "API for managing employee work journeys, including time tracking and activity monitoring.",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Support Team",
                    Email = "support@example.com"
                },
                License = new OpenApiLicense
                {
                    Name = "Example License",
                    Url = new Uri("https://example.com/license")
                }
            });
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