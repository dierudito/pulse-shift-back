using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;

namespace dm.PulseShift.bff.Endpoints.TimeEntries;

public class TimeEntryTodaysDurationEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/duration/today", HandleAsync)
            .WithName("TimeEntryTodaysDuration")
            .WithTags("Time Entry Today´s Duration")
            .Produces<Response<GetTodaysDurationResponseViewModel>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Get today's time entry duration");

    private static async Task<IResult> HandleAsync(ITimeEntryAppService appService)
    {
        var response = await appService.GetTodaysDurationAsync();
        return ResponseResult<GetTodaysDurationResponseViewModel>.CreateResponse(response);
    }
}
