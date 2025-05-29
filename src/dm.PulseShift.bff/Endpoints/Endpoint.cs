using dm.PulseShift.bff.Endpoints.DaysOff;
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
            .MapEndpoint<TimeEntryClockEndpoint>()
            .MapEndpoint<TimeEntryClockWithDateEndpoint>()
            .MapEndpoint<TimeEntryTodaysDurationEndpoint>()
            .MapEndpoint<TimeEntryDurationEndpoint>()
            .MapEndpoint<TimeEntryPeriodDurationEndpoint>()
            .MapEndpoint<TimeEntryScheduleByDateEndpoint>();

        endpoints.MapGroup(ApiConfigurations.RouterDayOff)
            .WithTags("Day Off")
            .MapEndpoint<CreateDayOffEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
