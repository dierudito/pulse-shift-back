using dm.PulseShift.bff.Endpoints.Activities;
using dm.PulseShift.bff.Endpoints.Charts;
using dm.PulseShift.bff.Endpoints.DaysOff;
using dm.PulseShift.bff.Endpoints.Reports;
using dm.PulseShift.bff.Endpoints.TimeEntries;
using dm.PulseShift.bff.Endpoints.WorkSummary;
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
            .MapEndpoint<TimeEntryScheduleByDateEndpoint>()
            .MapEndpoint<TimeEntriesTodaysEndpoint>()
            ;

        endpoints.MapGroup(ApiConfigurations.RouterDayOff)
            .WithTags("Day Off")
            .MapEndpoint<CreateDayOffEndpoint>();

        endpoints.MapGroup(ApiConfigurations.RouterActivity)
            .WithTags("Activity")
            .MapEndpoint<CreateActivityEndpoint>()
            .MapEndpoint<StartActivityEndpoint>()
            .MapEndpoint<FinishActivityEndpoint>()
            .MapEndpoint<GetActivityWorkDetailsByIdEndpoint>()
            .MapEndpoint<GetActivityWorkDetailsByCardCodeEndpoint>()
            .MapEndpoint<GetActivitiesSummaryEndpoint>()
            .MapEndpoint<AddRetroactiveActivityPeriodEndpoint>()
            .MapEndpoint<GetActivitiesPaginatedEndpoint>()
            ;

        endpoints.MapGroup(ApiConfigurations.RouterWorkSummary)
            .WithTags("Work Summary")
            .MapEndpoint<GetTotalWorkedHoursInRangeEndpoint>();

        endpoints.MapGroup(ApiConfigurations.RouterReports)
            .WithTags("Reports")
            .MapEndpoint<GetPeriodReportEndpoint>();

        endpoints.MapGroup(ApiConfigurations.RouterCharts)
            .WithTags("Charts")
            .MapEndpoint<GetTopActivitiesChartEndpoint>()
            .MapEndpoint<GetProductivityByDayEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
