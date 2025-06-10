using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;

namespace dm.PulseShift.bff.Endpoints.TimeEntries;

public class TimeEntriesTodaysEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/today", HandleAsync)
            .WithName("TodaysTimeEntries")
            .WithTags("Time Entry")
            .Produces<Response<IEnumerable<GetTimeEntriesPerDay>>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Get today's time entries");

    private static async Task<IResult> HandleAsync(ITimeEntryAppService appService)
    {
        var response = await appService.GetTimeEntriesPerDayAsync(DateOnly.FromDateTime(DateTime.Now));
        return ResponseResult<IEnumerable<GetTimeEntriesPerDay>>.CreateResponse(response);
    }
}