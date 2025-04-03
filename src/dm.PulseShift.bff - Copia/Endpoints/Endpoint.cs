using dm.PulseShift.bff.Endpoints.TimeEntries;
using dm.PulseShift.bff.Extensions;
using dm.PulseShift.Infra.CrossCutting.Shared;

namespace dm.PulseShift.bff.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("/api/v1");
        app.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => Results.Ok("Healthy"));

        endpoints.MapGroup(ApiConfigurations.RouterTimeEntry)
            .WithTags("Time Entry")
            .MapEndpoint<TimeEntryClockEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
